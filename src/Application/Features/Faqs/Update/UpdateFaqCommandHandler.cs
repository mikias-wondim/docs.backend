using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Faqs;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Faqs.Update;

internal sealed class UpdateFaqCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<UpdateFaqCommand>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public async Task<Result> Handle(UpdateFaqCommand command, CancellationToken cancellationToken)
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

        Faq? faq = await context.Faqs
            .Include(f => f.Page)
            .ThenInclude(p => p.Section)
            .ThenInclude(s => s.Project)
            .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(f => f.Id == command.FaqId, cancellationToken);

        if (faq is null)
        {
            return Result.Failure(FaqErrors.NotFound(command.FaqId));
        }

        if (faq.Page.Section.Project.OwnerId != currentUserId &&
            !faq.Page.Section.Project.Members.Any(m => m.UserId == currentUserId && m.CanWrite()))
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        string updatedBy = $"{user.FullName} ({user.Id})";

        faq.Update(
            command.Question,
            command.Answer,
            updatedBy,
            DateTimeProvider.UtcNow);
        
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
