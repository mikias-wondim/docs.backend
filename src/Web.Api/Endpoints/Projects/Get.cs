using Application.Abstractions.Messaging;
using Application.Features.Projects;
using Application.Features.Projects.Get;
using Domain.Projects;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Projects;

internal sealed class Get : IEndpoint
{
    private sealed record QueryParams(
        string? Name = null,
        ProjectVisibility? Visibility = null,
        string? SortBy = "createdAt",
        string? SortOrder = "desc",
        int Page = 1,
        int PageSize = 20);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("projects", async (
                [AsParameters] QueryParams queryParams,
                IQueryHandler<GetProjectsQuery, List<ProjectResponse>> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var query = new GetProjectsQuery(
                    queryParams.Name,
                    queryParams.Visibility,
                    queryParams.SortBy,
                    queryParams.SortOrder,
                    queryParams.Page,
                    queryParams.PageSize
                );

                Result<List<ProjectResponse>> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Projects);
    }
}
