using Application.Abstractions.Messaging;
using Application.Features.Projects.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Projects;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("projects/{projectId:guid}", async (
                Guid projectId,
                ICommandHandler<DeleteProjectCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteProjectCommand(projectId);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithTags(Tags.Projects)
            .RequireAuthorization();
    }
}
