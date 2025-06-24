using Application.Abstractions.Messaging;
using Application.Features.Projects;
using Application.Features.Projects.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Projects;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("projects/{projectId:guid}", async (
                Guid projectId,
                IQueryHandler<GetProjectByIdQuery, ProjectResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetProjectByIdQuery(projectId);

                Result<ProjectResponse> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Projects);
    }
}
