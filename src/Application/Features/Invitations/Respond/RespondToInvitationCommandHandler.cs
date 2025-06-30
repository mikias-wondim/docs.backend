using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Invitations;
using Domain.ProjectMembers;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Invitations.Respond;

internal sealed class RespondToInvitationCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<RespondToInvitationCommand, bool>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public async Task<Result<bool>> Handle(RespondToInvitationCommand command, CancellationToken cancellationToken)
    {
        Guid currentUserId;

        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<bool>(UserErrors.Unauthorized);
        }

        User? currentUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (currentUser is null)
        {
            return Result.Failure<bool>(UserErrors.NotFound(currentUserId));
        }

        DateTime now = DateTimeProvider.GetNow;

        Invitation invitation = await context.Invitations
            .FirstOrDefaultAsync(i => i.Id.ToString() == command.Token, cancellationToken);

        if (invitation is null)
        {
            return Result.Failure<bool>(
                InvitationErrors.NotFound(command.Token));
        }

        if (invitation.InvitedUserId != currentUserId)
        {
            return Result.Failure<bool>(UserErrors.Forbidden);
        }

        if (!invitation.IsPending())
        {
            return Result.Failure<bool>(InvitationErrors.AlreadyHandled);
        }

        if (invitation.IsExpired(now))
        {
            return Result.Failure<bool>(InvitationErrors.Expired);
        }

        if (command.Accept)
        {
            invitation.Accept(now);

            bool alreadyMember = await context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == invitation.ProjectId && pm.UserId == currentUserId, cancellationToken);

            if (alreadyMember)
            {
                return Result.Failure<bool>(ProjectMemberErrors.AlreadyExists(invitation.ProjectId, currentUserId));
            }

            // Add the user as a project member
            string createdBy = $"Invitation From: {invitation.InvitedByUserId}";

            var newMember = new ProjectMember(
                id: Guid.NewGuid(),
                projectId: invitation.ProjectId,
                userId: invitation.InvitedUserId,
                role: invitation.Role,
                createdBy: createdBy,
                now
            );

            context.ProjectMembers.Add(newMember);
        }
        else
        {
            invitation.Reject(now);
        }
        
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
