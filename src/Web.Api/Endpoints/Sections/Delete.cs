using Application.Abstractions.Messaging;
using Application.Features.Sections.Delete;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Sections;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("sections/{sectionId:guid}", async (
                [FromRoute] Guid sectionId,
                ICommandHandler<DeleteSectionCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new DeleteSectionCommand(sectionId);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            }).WithTags(Tags.Sections)
            .RequireAuthorization();
    }
}
