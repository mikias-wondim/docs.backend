using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.ProjectMembers.Add;

internal sealed class AddProjectMemberCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<AddProjectMemberCommand>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public async Task<Result> Handle(AddProjectMemberCommand command, CancellationToken cancellationToken)
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
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result.Failure<Guid>(ProjectErrors.NotFound(command.ProjectId));
        }

        bool isOwner = project.OwnerId == currentUserId;
        bool isAdmin = project.Members.Any(m => m.UserId == currentUserId && m.Role == ProjectRole.Admin);

        if (!isOwner && !isAdmin)
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        bool alreadyMember = project.Members.Any(m => m.UserId == command.UserId);
        if (alreadyMember)
        {
            return Result.Failure(ProjectMemberErrors.AlreadyExists(command.ProjectId, command.UserId));
        }

        string createdBy = $"{currentUser.FirstName} {currentUser.LastName} ({currentUser.Id})";

        var newMember = new ProjectMember(
            id: Guid.NewGuid(),
            projectId: command.ProjectId,
            userId: command.UserId,
            role: command.Role,
            createdBy: createdBy,
            DateTimeProvider.GetNow
        );

        context.ProjectMembers.Add(newMember);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
