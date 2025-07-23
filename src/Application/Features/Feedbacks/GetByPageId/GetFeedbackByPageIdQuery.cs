using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Feedbacks.GetByPageId;

public sealed record GetFeedbackByPageIdQuery(
    Guid PageId,
    string? Comment = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc",
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<FeedbackResponse>>;
