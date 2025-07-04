using Application.Abstractions.Messaging;
using Application.Features.Pages.Delete;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Pages;

internal sealed class Delete: IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("pages/{pageId:guid}", async (
                [FromRoute] Guid pageId,
                ICommandHandler<DeletePageCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new DeletePageCommand(pageId);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            }).WithTags(Tags.Pages)
            .RequireAuthorization();
    }
}
