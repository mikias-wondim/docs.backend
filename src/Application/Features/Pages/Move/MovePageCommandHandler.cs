using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Pages;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Pages.Move;

internal sealed class MovePageCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<MovePageCommand>
{
    public async Task<Result> Handle(MovePageCommand command, CancellationToken cancellationToken)
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

        User? user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(currentUserId));
        }

        Page? page = await context.Pages
            .Include(p => p.Section)
                .ThenInclude(s => s.Project)
                    .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == command.PageId, cancellationToken);

        if (page is null)
        {
            return Result.Failure(PageErrors.NotFound(command.PageId));
        }

        bool isOwner = page.Section.Project.OwnerId == currentUserId;
        bool canWrite = page.Section.Project.Members.Any(m => m.UserId == currentUserId && m.CanWrite());

        if (!isOwner && !canWrite)
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        List<Page> siblingPages = await context.Pages
            .Where(p => p.SectionId == page.SectionId && p.ParentPageId == command.NewParentPageId && p.Id != page.Id)
            .OrderBy(p => p.Order)
            .ToListAsync(cancellationToken);

        siblingPages.Insert(command.NewIndex, page);

        const decimal step = 1.0m;
        for (int i = 0; i < siblingPages.Count; i++)
        {
            siblingPages[i].SetOrder(i * step + 1, GetAuditName(user), dateTimeProvider.UtcNow);
        }

        page.SetParent(command.NewParentPageId, GetAuditName(user), dateTimeProvider.UtcNow);

        bool shouldReindex = siblingPages.Any(p => DecimalPlaces(p.Order) > 4);
        if (shouldReindex)
        {
            for (int i = 0; i < siblingPages.Count; i++)
            {
                siblingPages[i].SetOrder(i + 1, GetAuditName(user), dateTimeProvider.UtcNow);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static string GetAuditName(User user) => $"{user.FirstName} {user.LastName} ({user.Id})";

    private static int DecimalPlaces(decimal number)
    {
        number = Math.Abs(number);
        int[] bits = decimal.GetBits(number);
        return (bits[3] >> 16) & 0xFF;
    }
}
