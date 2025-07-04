using Application.Abstractions.Messaging;
using Application.Features.Faqs.Delete;
using Application.Features.Pages.Delete;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Faqs;

internal sealed class Delete: IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("faqs/{faqId:guid}", async (
                [FromRoute] Guid faqId,
                ICommandHandler<DeleteFaqCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new DeleteFaqCommand(faqId);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            }).WithTags(Tags.Faqs)
            .RequireAuthorization();
    }
}
