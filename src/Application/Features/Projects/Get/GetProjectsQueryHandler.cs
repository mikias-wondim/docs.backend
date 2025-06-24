using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Projects;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Projects.Get;

internal sealed class GetProjectsQueryHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    IMapper mapper)
    : IQueryHandler<GetProjectsQuery, List<ProjectResponse>>
{
    public async Task<Result<List<ProjectResponse>>> Handle(GetProjectsQuery query, CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            currentUserId = Guid.Empty;
        }
        
        IQueryable<Project> projectsQuery = dbContext.Projects.AsNoTracking()
            .Where(p => p.Visibility == ProjectVisibility.Public || p.OwnerId == currentUserId);

        // Filter
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            projectsQuery = projectsQuery.Where(p => p.Name.Contains(query.Name));
        }

        if (query.Visibility is not null)
        {
            projectsQuery = projectsQuery.Where(p => p.Visibility == query.Visibility);
        }

        // Sorting
        projectsQuery = (query.SortBy?.ToLower(System.Globalization.CultureInfo.CurrentCulture),
                query.SortOrder?.ToLower(System.Globalization.CultureInfo.CurrentCulture)) switch
            {
                ("name", "asc") => projectsQuery.OrderBy(p => p.Name),
                ("name", "desc") => projectsQuery.OrderByDescending(p => p.Name),
                ("updatedat", "asc") => projectsQuery.OrderBy(p => p.UpdatedAt),
                ("updatedat", "desc") => projectsQuery.OrderByDescending(p => p.UpdatedAt),
                ("createdat", "asc") => projectsQuery.OrderBy(p => p.CreatedAt),
                _ => projectsQuery.OrderByDescending(p => p.CreatedAt)
            };

        // Pagination
        int skip = (query.Page - 1) * query.PageSize;
        List<Project> pagedProjects = await projectsQuery
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        // Mapping
        List<ProjectResponse>? projectResponses = mapper.Map<List<ProjectResponse>>(pagedProjects);

        return Result.Success(projectResponses);
    }
}
