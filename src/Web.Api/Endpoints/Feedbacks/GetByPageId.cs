using Application.Abstractions.Messaging;
using Application.Features.Feedbacks;
using Application.Features.Feedbacks.GetByPageId;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Feedbacks;

internal sealed class GetByPageId : IEndpoint
{
    private sealed record QueryParams(
        string? Comment = null,
        string? SortBy = "createdAt",
        string? SortOrder = "desc",
        int Page = 1,
        int PageSize = 20);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("pages/{pageId:guid}/feedbacks", async (
            [FromRoute] Guid pageId,
            [AsParameters] QueryParams queryParams,
            IQueryHandler<GetFeedbackByPageIdQuery, PagedResult<FeedbackResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetFeedbackByPageIdQuery(
                pageId,
                queryParams.Comment,
                queryParams.SortBy,
                queryParams.SortOrder,
                queryParams.Page,
                queryParams.PageSize
            );

            Result<PagedResult<FeedbackResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Feedback);
    }
}
