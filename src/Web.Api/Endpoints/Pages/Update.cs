using Application.Abstractions.Messaging;
using Application.Features.Pages.Update;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Pages;

internal sealed class Update : IEndpoint
{
    private sealed record Request(
        string Title,
        string? ContentMd,
        List<string> Tags);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("pages/{pageId:guid}", async (
                [FromRoute] Guid pageId,
                [FromBody] Request request,
                ICommandHandler<UpdatePageCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new UpdatePageCommand(
                    pageId,
                    request.Title,
                    request.ContentMd,
                    request.Tags);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            }).WithTags(Tags.Pages)
            .RequireAuthorization();
    }
}
