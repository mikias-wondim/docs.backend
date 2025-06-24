using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Projects.UpdateOverview;

internal sealed class UpdateProjectOverviewCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<UpdateProjectOverviewCommand, Guid>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public async Task<Result<Guid>> Handle(UpdateProjectOverviewCommand command, CancellationToken cancellationToken)
    {
        Guid userId;
        try
        {
            userId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<Guid>(UserErrors.Unauthorized());       
        }
        
        Project? project = await context.Projects
            .SingleOrDefaultAsync(p => p.Id == command.ProjectId, cancellationToken);

        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(userContext.UserId));
        }

        if (project is null)
        {
            return Result.Failure<Guid>(ProjectErrors.NotFound(command.ProjectId));
        }

        if (project.OwnerId != userId)
        {
            return Result.Failure<Guid>(ProjectErrors.Unauthorized(command.ProjectId));
        }
        
        string updatedBy = $"{user.FirstName} {user.LastName} ({user.Id})";
        
        project.UpdateOverview(
            command.Overview,
            updatedBy,
            DateTimeProvider.GetNow);
        
        await context.SaveChangesAsync(cancellationToken);

        return command.ProjectId;
    }
}
