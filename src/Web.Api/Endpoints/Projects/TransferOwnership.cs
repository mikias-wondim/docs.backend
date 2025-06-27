using Application.Abstractions.Messaging;
using Application.Features.Projects.TransferOwnership;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Projects;

internal sealed class TransferOwnership: IEndpoint
{
    private sealed record Request(Guid NewOwnerId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("projects/{projectId:guid}/transfer-ownership", async (
                Guid projectId,
                [FromBody] Request request,
                ICommandHandler<TransferProjectOwnershipCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var command =
                    new TransferProjectOwnershipCommand(projectId, request.NewOwnerId);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithTags(Tags.Projects)
            .RequireAuthorization();
    }
}
