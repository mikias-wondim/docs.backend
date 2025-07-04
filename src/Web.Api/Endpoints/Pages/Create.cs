using Application.Abstractions.Messaging;
using Application.Features.Pages.Create;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Pages;

internal sealed class Create : IEndpoint
{
    private sealed record Request(
        string Title,
        Guid? ParentPageId,
        string? ContentMd,
        List<string> Tags);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/sections/{sectionId:guid}/pages", async (
                [FromRoute] Guid sectionId,
                [FromBody] Request request,
                ICommandHandler<CreatePageCommand, Guid> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new CreatePageCommand(
                    sectionId,
                    request.Title,
                    request.ParentPageId,
                    request.ContentMd,
                    request.Tags);

                Result<Guid> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            }).WithTags(Tags.Pages)
            .RequireAuthorization();
    }
}
