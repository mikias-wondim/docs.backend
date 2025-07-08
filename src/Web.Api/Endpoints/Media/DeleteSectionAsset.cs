using Application.Abstractions.Messaging;
using Application.Features.Media.Delete;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Web.Api.Endpoints.Media;

internal sealed class DeleteSectionAsset : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("sections/{sectionId:guid}/assets", async (
                [FromRoute] Guid sectionId,
                [FromQuery] string relativePath,
                ICommandHandler<DeleteSectionAssetCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new DeleteSectionAssetCommand(sectionId, relativePath);
                Result result = await handler.Handle(command, cancellationToken);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithTags(Tags.Media)
            .RequireAuthorization();
    }
}
