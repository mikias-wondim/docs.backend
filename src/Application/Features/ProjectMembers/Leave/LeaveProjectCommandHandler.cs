using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.ProjectMembers.Leave;

internal sealed class LeaveProjectCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<LeaveProjectCommand>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result> Handle(LeaveProjectCommand command, CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure(UserErrors.Unauthorized);
        }
        
        User? currentUser = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (currentUser is null)
        {
            return Result.Failure(UserErrors.NotFound(currentUserId));
        }

        Project? project = await context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result.Failure(ProjectErrors.NotFound(command.ProjectId));
        }

        if (project.OwnerId == currentUserId)
        {
            return Result.Failure(ProjectMemberErrors.OwnerCannotLeave(command.ProjectId));
        }

        ProjectMember? membership = await context.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == command.ProjectId && pm.UserId == currentUserId,
                cancellationToken);

        if (membership is null)
        {
            return Result.Failure(ProjectMemberErrors.NotAMember(command.ProjectId, currentUserId));
        }
        
        string updatedBy = $"{currentUser.FirstName} {currentUser.LastName} ({currentUser.Id})";
        
        membership.Delete(DateTimeProvider.GetNow, updatedBy);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
