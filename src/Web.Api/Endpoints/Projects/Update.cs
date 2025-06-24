using Application.Abstractions.Messaging;
using Application.Features.Projects.Update;
using Domain.Projects;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Projects;

internal sealed class Update : IEndpoint
{
    private sealed record Request(string Name, string? Description, ProjectVisibility Visibility);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("projects/{projectId:guid}/update", async (
                Guid projectId,
                [FromBody] Request request,
                ICommandHandler<UpdateProjectCommand, Guid> handler,
                CancellationToken cancellationToken) =>
            {
                var command =
                    new UpdateProjectCommand(projectId, request.Name, request.Description, request.Visibility);

                Result<Guid> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Projects)
            .RequireAuthorization();
    }
}
