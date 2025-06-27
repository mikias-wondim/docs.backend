using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Projects.Delete;

internal sealed class DeleteProjectCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<DeleteProjectCommand>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public async Task<Result> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        Guid userId;
        try
        {
            userId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure(UserErrors.Unauthorized);
        }

        Project? project = await context.Projects
            .SingleOrDefaultAsync(p => p.Id == command.ProjectId, cancellationToken);

        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userContext.UserId));
        }

        if (project is null)
        {
            return Result.Failure(ProjectErrors.NotFound(command.ProjectId));
        }

        if (project.OwnerId != userId)
        {
            return Result.Failure<Guid>(UserErrors.Forbidden);
        }

        string deletedBy = $"{user.FirstName} {user.LastName} ({user.Id})";

        project.Delete(DateTimeProvider.GetNow, deletedBy);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
