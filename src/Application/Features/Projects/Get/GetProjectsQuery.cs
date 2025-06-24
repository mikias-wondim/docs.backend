using Application.Abstractions.Messaging;
using Domain.Projects;

namespace Application.Features.Projects.Get;

public record GetProjectsQuery(
    string? Name = null,
    ProjectVisibility? Visibility = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc",
    int Page = 1,
    int PageSize = 20
) : IQuery<List<ProjectResponse>>;
