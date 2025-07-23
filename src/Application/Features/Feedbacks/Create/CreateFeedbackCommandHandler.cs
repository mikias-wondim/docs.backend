using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Feedbacks;
using Domain.Pages;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Feedbacks.Create;

internal sealed class CreateFeedbackCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider
    ): ICommandHandler<CreateFeedbackCommand>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result> Handle(CreateFeedbackCommand command, CancellationToken cancellationToken)
    {
        Page? page = await context.Pages
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == command.PageId, cancellationToken);

        if (page is null)
        {
            return Result.Failure(PageErrors.NotFound(command.PageId));
        }
        
        var feedback = new Feedback(
            id: Guid.NewGuid(),
            pageId: command.PageId,
            comment: command.Comment,
            rating: command.Rating,
            createdAt: DateTimeProvider.GetNow
        );
        
        context.Feedbacks.Add(feedback);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
