using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Pages;
using Domain.Sections;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Pages.Create;

internal sealed class CreatePageCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreatePageCommand, Guid>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result<Guid>> Handle(CreatePageCommand command, CancellationToken cancellationToken)
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
        
        User? user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(currentUserId));       
        }
        
        Section? section = await context.Sections
            .AsNoTracking()
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
        
        Page lastSiblingPage = await context.Pages
            .Where(p => p.SectionId == command.SectionId && p.ParentPageId == command.ParentPageId)
            .OrderByDescending(p => p.Order)
            .FirstOrDefaultAsync(cancellationToken);
        
        decimal newOrder = lastSiblingPage is null
            ? 1
            : lastSiblingPage.Order + 1;
        
        string createdBy = $"{user.FirstName} {user.LastName} ({user.Id})";

        var page = new Page(
            Guid.NewGuid(),
            command.SectionId,
            command.Title,
            newOrder,
            createdBy,
            DateTimeProvider.UtcNow,
            command.ParentPageId,
            command.ContentMd,
            command.Tags
        );

        await context.Pages.AddAsync(page, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return page.Id;
    }
}
