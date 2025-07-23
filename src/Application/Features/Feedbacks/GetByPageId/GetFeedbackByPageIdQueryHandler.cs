using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using AutoMapper;
using Domain.Feedbacks;

namespace Application.Features.Feedbacks.GetByPageId;

internal sealed class GetFeedbackByPageIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
) : IQueryHandler<GetFeedbackByPageIdQuery, PagedResult<FeedbackResponse>>
{
    public async Task<Result<PagedResult<FeedbackResponse>>> Handle(GetFeedbackByPageIdQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Feedback> feedbackQuery = context.Feedbacks
            .AsNoTracking()
            .Where(f => f.PageId == query.PageId);

        if (!string.IsNullOrWhiteSpace(query.Comment))
        {
            string lowered = query.Comment.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            feedbackQuery = feedbackQuery.Where(f => 
                f.Comment != null && 
                EF.Functions.Like(f.Comment, $"%{lowered}%"));
        }

        feedbackQuery = query.SortBy?.ToLower(System.Globalization.CultureInfo.CurrentCulture) switch
        {
            "rating" => query.SortOrder == "asc"
                ? feedbackQuery.OrderBy(f => f.Rating)
                : feedbackQuery.OrderByDescending(f => f.Rating),

            _ => query.SortOrder == "asc"
                ? feedbackQuery.OrderBy(f => f.CreatedAt)
                : feedbackQuery.OrderByDescending(f => f.CreatedAt)
        };

        int totalCount = await feedbackQuery.CountAsync(cancellationToken);

        List<Feedback> feedbacks = await feedbackQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var result = new PagedResult<FeedbackResponse>
        {
            Items = mapper.Map<FeedbackResponse[]>(feedbacks),
            TotalCount = totalCount,
            PageNumber = query.Page,
            PageSize = query.PageSize
        };

        return Result.Success(result);
    }
}
