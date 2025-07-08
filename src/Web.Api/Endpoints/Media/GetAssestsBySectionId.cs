using Application.Abstractions.Messaging;
using Application.Features.Media;
using Application.Features.Media.GetBySectionId;
using SharedKernel;

namespace Web.Api.Endpoints.Media;

internal sealed class GetSectionAssets : IEndpoint
{
    private sealed record QueryParams(
        string? SortBy = "createdAt",
        string? SortOrder = "desc",
        int Page = 1,
        int PageSize = 20);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("sections/{sectionId:guid}/assets/all", async (
                Guid sectionId,
                [AsParameters] QueryParams queryParams,
                IQueryHandler<GetAssetsSectionByIdQuery, PagedResult<MediaAssetResponse>> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var query = new GetAssetsSectionByIdQuery(
                    sectionId,
                    queryParams.SortBy,
                    queryParams.SortOrder,
                    queryParams.Page,
                    queryParams.PageSize);
                Result<PagedResult<MediaAssetResponse>> result = await handler.Handle(query, cancellationToken);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithTags(Tags.Media)
            .RequireAuthorization();
    }
}
