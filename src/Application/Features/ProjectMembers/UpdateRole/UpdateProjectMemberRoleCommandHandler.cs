using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.ProjectMembers.UpdateRole;

internal sealed class UpdateProjectMemberRoleCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<UpdateProjectMemberRoleCommand>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public async Task<Result> Handle(UpdateProjectMemberRoleCommand command, CancellationToken cancellationToken)
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
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result.Failure(ProjectErrors.NotFound(command.ProjectId));
        }

        bool isOwner = project.OwnerId == currentUserId;
        bool isAdmin = project.Members.Any(m => m.UserId == currentUserId && m.Role == ProjectRole.Admin);

        if (!isOwner && !isAdmin)
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        if (command.UserId == project.OwnerId)
        {
            return Result.Failure(ProjectMemberErrors.CannotModifyOwner(project.Id));
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

        member.ChangeRole(command.NewRole, updatedBy, DateTimeProvider.GetNow);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
