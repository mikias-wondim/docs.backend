using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Projects.Create;

internal sealed class CreateProjectCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
    ): ICommandHandler<CreateProjectCommand, Guid>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result<Guid>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<Guid>(UserErrors.Unauthorized);
        }
        
        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
        
        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(currentUserId));
        }
        
        string createdBy = $"{user.FirstName} {user.LastName} ({user.Id})";
        
        var project = new Project(
            Guid.NewGuid(),
            currentUserId,
            command.Name,
            command.Description,
            command.Visibility,
            DateTimeProvider.GetNow,
            createdBy);
        
        context.Projects.Add(project);

        await context.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}
