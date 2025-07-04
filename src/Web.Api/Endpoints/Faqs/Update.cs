using Application.Abstractions.Messaging;
using Application.Features.Faqs.Update;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Faqs;

internal sealed class Update : IEndpoint
{
    private sealed record Request(
        string Question,
        string Answer);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("faqs/{faqId:guid}", async (
                [FromRoute] Guid faqId,
                [FromBody] Request request,
                ICommandHandler<UpdateFaqCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new UpdateFaqCommand(
                    faqId,
                    request.Question,
                    request.Answer);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            }).WithTags(Tags.Faqs)
            .RequireAuthorization();
    }
}
