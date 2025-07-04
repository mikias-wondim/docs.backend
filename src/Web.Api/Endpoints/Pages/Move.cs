using Application.Abstractions.Messaging;
using Application.Features.Pages.Move;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Pages;

internal sealed class Move : IEndpoint
{
    private sealed record Request(
        Guid? NewParentPageId,
        int NewIndex);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("pages/{pageId:guid}/move", async (
                [FromRoute] Guid pageId,
                [FromBody] Request request,
                ICommandHandler<MovePageCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new MovePageCommand(
                    pageId,
                    request.NewParentPageId,
                    request.NewIndex);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithTags(Tags.Pages)
            .RequireAuthorization();
    }
}
