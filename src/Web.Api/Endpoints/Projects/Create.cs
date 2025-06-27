using Application.Abstractions.Messaging;
using Application.Features.Projects.Create;
using Domain.Projects;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Projects;

internal sealed class Create : IEndpoint
{
    private sealed record Request(
        string Name,
        string? Description,
        ProjectVisibility Visibility);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("projects", async (
                [FromBody]Request request,
                ICommandHandler<CreateProjectCommand, Guid> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateProjectCommand(request.Name, request.Description,
                    request.Visibility);

                Result<Guid> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Projects)
            .RequireAuthorization();
    }
}
