using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services.Email;
using Domain.Invitations;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Invitations.Send;

internal sealed class SendInvitationCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IEmailInvitationService emailInvitationService,
    IDateTimeProvider dateTimeProvider): ICommandHandler<SendInvitationCommand, bool>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public async Task<Result<bool>> Handle(SendInvitationCommand command, CancellationToken cancellationToken)
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

        Project? project = await context.Projects
            .AsNoTracking()
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result.Failure<bool>(ProjectErrors.NotFound(command.ProjectId));
        }

        bool isOwner = project.OwnerId == currentUserId;
        bool isAdmin = project.Members.Any(m => m.UserId == currentUserId && m.Role == ProjectRole.Admin);

        if (!isOwner && !isAdmin)
        {
            return Result.Failure<bool>(UserErrors.Forbidden);
        }

        bool alreadyMember = project.OwnerId == command.UserId || project.Members.Any(m => m.UserId == command.UserId);
        if (alreadyMember)
        {
            return Result.Failure<bool>(ProjectMemberErrors.AlreadyExists(command.ProjectId, command.UserId));
        }

        User? invitedUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (invitedUser is null)
        {
            return Result.Failure<bool>(UserErrors.NotFound(command.UserId));
        }

        bool alreadyInvited = await context.Invitations
            .Where(i => i.InvitedUserId == command.UserId &&
                        i.ProjectId == command.ProjectId &&
                        i.Status == InvitationStatus.Pending &&
                        i.ExpiresAt > DateTimeProvider.GetNow)
            .AnyAsync(cancellationToken);

        if (alreadyInvited)
        {
            return Result.Failure<bool>(InvitationErrors.DuplicateInvitation);
        }

        string createdBy = $"{currentUser.FirstName} {currentUser.LastName} ({currentUser.Id})";

        var newInvitation = new Invitation(
            Guid.NewGuid(),
            command.ProjectId,
            command.UserId,
            currentUserId,
            command.Role,
            DateTimeProvider.GetNow,
            DateTimeProvider.GetNow.AddDays(7),
            createdBy);
        try
        {
            await emailInvitationService.SendAsync(
                newInvitation.Id.ToString(),
                invitedUser.Email,
                invitedUser.FirstName,
                currentUser.FirstName,
                project.Name,
                newInvitation.SentAt,
                newInvitation.ExpiresAt,
                newInvitation.Role,
                cancellationToken);
        }
        catch (Exception)
        {
            return Result.Failure<bool>(InvitationErrors.FailedToSendEmail);
        }

        context.Invitations.Add(newInvitation);
        bool isSent = await context.SaveChangesAsync(cancellationToken) > 0;

        return isSent;
    }
}
