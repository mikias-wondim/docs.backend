using Application.Abstractions.Messaging;
using Application.Features.Sections.ChangeDefaultPage;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Sections;

internal sealed class ChangeDefaultPage : IEndpoint
{
    private sealed record Request(
        Guid DefaultPageId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("sections/{sectionId:guid}/default", async (
                [FromRoute] Guid sectionId,
                [FromBody] Request request,
                ICommandHandler<ChangeDefaultPageCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new ChangeDefaultPageCommand(
                    sectionId,
                    request.DefaultPageId);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            }).WithTags(Tags.Sections)
            .RequireAuthorization();
    }
}
