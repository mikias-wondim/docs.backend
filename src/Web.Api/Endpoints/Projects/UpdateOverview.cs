using Application.Abstractions.Messaging;
using Application.Features.Projects.UpdateOverview;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Projects;

internal sealed class UpdateOverview : IEndpoint
{
    private sealed record Request(string Overview);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("projects/{projectId:guid}/overview", async (
                Guid projectId,
                [FromBody] Request request,
                ICommandHandler<UpdateProjectOverviewCommand, Guid> handler,
                CancellationToken cancellationToken) =>
            {
                var command =
                    new UpdateProjectOverviewCommand(projectId, request.Overview);

                Result<Guid> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Projects)
            .RequireAuthorization();
    }
}
