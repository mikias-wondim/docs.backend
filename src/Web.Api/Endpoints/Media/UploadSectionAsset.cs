using Application.Abstractions.Messaging;
using Application.Features.Media;
using Application.Features.Media.Upload;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Media;

internal sealed class UploadSectionAsset : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("sections/{sectionId:guid}/assets/upload", async (
                Guid sectionId,
                IFormFile file,
                string? altName,
                ICommandHandler<UploadSectionAssetCommand, MediaAssetResponse> sender,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new UploadSectionAssetCommand(sectionId, file, altName);

                Result<MediaAssetResponse> result = await sender.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .Accepts<IFormFile>("multipart/form-data")
            .DisableAntiforgery()
            .RequireAuthorization()
            .WithTags(Tags.Media);
    }
}
