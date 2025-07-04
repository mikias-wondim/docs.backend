using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Sections;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Sections.ChangeDefaultPage;

internal sealed class ChangeDefaultPageCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext): ICommandHandler<ChangeDefaultPageCommand>
{
    public async Task<Result> Handle(ChangeDefaultPageCommand command, CancellationToken cancellationToken)
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
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(currentUserId));       
        }
        
        Section? section = await context.Sections
            .Include(s => s.AllowedUsers) 
            .Include(s => s.Project)
            .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(s => s.Id == command.SectionId, cancellationToken);

        if (section is null)
        {
            return Result.Failure<Guid>(SectionErrors.NotFound(command.SectionId));
        }

        bool isOwner = section.Project.OwnerId == currentUserId;
        bool canWrite = section.Project.Members.Any(m => m.UserId == currentUserId && m.CanWrite());

        if (!isOwner && !canWrite)
        {
            return Result.Failure<Guid>(UserErrors.Forbidden);
        }
        
        section.SetDefaultPage(command.PageId);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
