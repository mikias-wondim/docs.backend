using Application.Abstractions.Messaging;
using Application.Features.Pages;
using Application.Features.Pages.GetBySectionId;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Pages;

internal sealed class GetBySectionId: IEndpoint
{
    private sealed record QueryParams(
        string? Query= null,
        string? Tag = null);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("sections/{sectionId:guid}/pages", async (
            [FromRoute] Guid sectionId,
            [AsParameters] QueryParams queryParams,
            IQueryHandler<GetPagesBySectionIdQuery, List<PageSummaryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPagesBySectionIdQuery(sectionId, queryParams.Query, queryParams.Tag);

            Result<List<PageSummaryResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Pages);
    }
}
