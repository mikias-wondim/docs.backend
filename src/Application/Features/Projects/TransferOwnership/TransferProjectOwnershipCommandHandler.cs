using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Projects.TransferOwnership;

internal sealed class TransferProjectOwnershipCommandHandler(
    IApplicationDbContext context, 
    IUserContext userContext, 
    IDateTimeProvider dateTimeProvider): ICommandHandler<TransferProjectOwnershipCommand>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result> Handle(TransferProjectOwnershipCommand command, CancellationToken cancellationToken)
    {
        Guid userId;
        try
        {
            userId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<Guid>(UserErrors.Unauthorized);       
        }
        
        Project? project = await context.Projects
            .AsNoTracking()
            .Include(p => p.Members)
            .SingleOrDefaultAsync(p => p.Id == command.ProjectId, cancellationToken);
        
        if (project is null)
        {
            return Result.Failure<Guid>(ProjectErrors.NotFound(command.ProjectId));
        }

        if (project.OwnerId != userId)
        {
            return Result.Failure<Guid>(UserErrors.Forbidden);
        }
        
        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(userContext.UserId));
        }

        string updatedBy = $"{user.FirstName} {user.LastName} ({user.Id})";
        
        ProjectMember? member = project.Members.SingleOrDefault(m => m.UserId == command.NewOwnerId);
        member?.Delete(DateTimeProvider.GetNow, updatedBy);
        
        project.TransferOwnership(
            command.NewOwnerId,
            updatedBy,
            DateTimeProvider.GetNow);
        
        
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
