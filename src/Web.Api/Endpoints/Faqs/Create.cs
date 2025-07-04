using Application.Abstractions.Messaging;
using Application.Features.Faqs.Create;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Faqs;

internal sealed class Create : IEndpoint
{
    private sealed record Request(
        string Question,
        string Answer);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/pages/{pageId:guid}/faqs", async (
                [FromRoute] Guid pageId,
                [FromBody] Request request,
                ICommandHandler<CreateFaqCommand, Guid> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new CreateFaqCommand(
                    pageId,
                    request.Question,
                    request.Answer);

                Result<Guid> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            }).WithTags(Tags.Faqs)
            .RequireAuthorization();
    }
}
