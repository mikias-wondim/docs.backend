using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.ProjectMembers.Remove;

internal sealed class RemoveProjectMemberCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<RemoveProjectMemberCommand>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result> Handle(RemoveProjectMemberCommand command, CancellationToken cancellationToken)
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
        
        bool isOwner = project.OwnerId == currentUserId;
        bool isAdmin = await context.ProjectMembers
            .AnyAsync(pm =>
                pm.ProjectId == command.ProjectId &&
                pm.UserId == currentUserId &&
                pm.Role == ProjectRole.Admin,
                cancellationToken);

        if (!isOwner && !isAdmin)
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        if (project.OwnerId == command.UserId)
        {
            return Result.Failure(ProjectMemberErrors.CannotRemoveOwner(command.ProjectId));
        }

        ProjectMember? member = await context.ProjectMembers
            .FirstOrDefaultAsync(pm =>
                pm.ProjectId == command.ProjectId &&
                pm.UserId == command.UserId,
                cancellationToken);

        if (member is null)
        {
            return Result.Failure(ProjectMemberErrors.NotAMember(command.ProjectId, command.UserId));
        }

        string updatedBy = $"{currentUser.FirstName} {currentUser.LastName} ({currentUser.Id})";
        
        member.Delete(DateTimeProvider.GetNow, updatedBy);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
