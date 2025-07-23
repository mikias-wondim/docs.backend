using Application.Abstractions.Messaging;
using Application.Features.Feedbacks.Create;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Feedbacks;

internal sealed class Create: IEndpoint
{
    private sealed record Request(
        ushort Rating,
        string? Comment);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("pages/{pageId:guid}/feedbacks", async (
            [FromRoute] Guid pageId,
            [FromBody] Request request,
            ICommandHandler<CreateFeedbackCommand> handler,
            CancellationToken cancellationToken
            ) =>
        {
            var command = new CreateFeedbackCommand(
                pageId,
                request.Rating,
                request.Comment);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Created, CustomResults.Problem);
        }).WithTags(Tags.Feedback);
    }
}
