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
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext
    ): ICommandHandler<CreateProjectCommand, Guid>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result<Guid>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (userContext.UserId != command.OwnerId)
            {
                return Result.Failure<Guid>(UserErrors.Forbidden);
            }
        }
        catch (ApplicationException)
        {
            return Result.Failure<Guid>(UserErrors.Forbidden);
        }
        
        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == command.OwnerId, cancellationToken);
        
        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.OwnerId));
        }
        
        string createdBy = $"{user.FirstName} {user.LastName} ({user.Id})";
        
        var project = new Project(
            Guid.NewGuid(),
            command.OwnerId,
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
