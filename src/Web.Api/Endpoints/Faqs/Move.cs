using Application.Abstractions.Messaging;
using Application.Features.Faqs.Move;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Faqs;

internal sealed class Move : IEndpoint
{
    private sealed record Request(int NewIndex);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("faqs/{faqId:guid}/move", async (
                [FromRoute] Guid faqId,
                [FromBody] Request request,
                ICommandHandler<MoveFaqCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new MoveFaqCommand(
                    faqId,
                    request.NewIndex);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithTags(Tags.Faqs)
            .RequireAuthorization();
    }
}
