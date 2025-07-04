using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Faqs;
using Domain.Pages;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Faqs.Create;

internal sealed class CreateFaqCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateFaqCommand, Guid>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public async Task<Result<Guid>> Handle(CreateFaqCommand command, CancellationToken cancellationToken)
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

        Page? page = await context.Pages
            .AsNoTracking()
            .Include(p => p.Section)
            .ThenInclude(s => s.Project)
            .ThenInclude(p => p.Members)
            .Include(p => p.Faqs)
            .FirstOrDefaultAsync(p => p.Id == command.PageId, cancellationToken);

        if (page is null)
        {
            return Result.Failure<Guid>(PageErrors.NotFound(command.PageId));
        }
        
        bool isOwner = page.Section.Project.OwnerId == currentUserId;
        bool canWrite = page.Section.Project.Members
            .Any(m => m.UserId == currentUserId && m.CanWrite());

        if (!isOwner && !canWrite)
        {
            return Result.Failure<Guid>(UserErrors.Forbidden);
        }

        decimal newOrder = page.Faqs.Any() ? page.Faqs.Max(f => f.Order) + 1 : 1;

        string createdBy = $"{user.FullName} ({user.Id})";

        var faq = new Faq(
            Guid.NewGuid(),
            command.PageId,
            command.Question,
            command.Answer,
            newOrder,
            createdBy,
            DateTimeProvider.UtcNow);

        context.Faqs.Add(faq);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(faq.Id);
    }
}
