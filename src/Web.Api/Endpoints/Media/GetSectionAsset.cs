using Application.Abstractions.Messaging;
using Application.Features.Media.Get;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Web.Api.Endpoints.Media;

internal sealed class GetSectionAsset : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("sections/{sectionId:guid}/assets", async (
                [FromRoute] Guid sectionId,
                [FromQuery] string relativePath,
                IQueryHandler<GetSectionAssetQuery, Stream> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var query = new GetSectionAssetQuery(sectionId, relativePath);
                Result<Stream> result = await handler.Handle(query, cancellationToken);

                return result.IsSuccess
                    ? Results.File(result.Value, contentType: "application/octet-stream", enableRangeProcessing: true)
                    : Results.NotFound(new { error = result.Error });
            })
            .WithTags(Tags.Media)
            .RequireAuthorization();
    }
}
