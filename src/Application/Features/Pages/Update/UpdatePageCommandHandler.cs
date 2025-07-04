using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Pages;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Pages.Update;

internal sealed class UpdatePageCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<UpdatePageCommand>
{
    public async Task<Result> Handle(UpdatePageCommand command, CancellationToken cancellationToken)
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
        bool canWrite = page.Section.Project.Members
            .Any(m => m.UserId == currentUserId && m.CanWrite());

        if (!isOwner && !canWrite)
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        string updatedBy = $"{user.FirstName} {user.LastName} ({user.Id})";
        DateTime now = dateTimeProvider.UtcNow;

        page.Update(
            command.Title,
            command.ContentMd,
            command.Tags,
            updatedBy,
            now
        );

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
