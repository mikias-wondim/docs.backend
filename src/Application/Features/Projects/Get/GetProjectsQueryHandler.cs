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
    : IQueryHandler<GetProjectsQuery, PagedResult<ProjectResponse>>
{
    public async Task<Result<PagedResult<ProjectResponse>>> Handle(GetProjectsQuery query,
        CancellationToken cancellationToken)
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

        IQueryable<Project> projectsQuery = dbContext.Projects
            .AsNoTracking()
            .Where(p => p.RecordStatus != RecordStatus.Deleted &&
                        (p.Visibility == ProjectVisibility.Public || p.OwnerId == currentUserId ||
                         p.Members.Any(m => m.UserId == currentUserId)));
        // Filter
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            string pattern = $"%{query.Name}%";
            projectsQuery = projectsQuery.Where(p => EF.Functions.Like(p.Name, pattern));
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

        // Total count before pagination
        int totalCount = await projectsQuery.CountAsync(cancellationToken);

        // Pagination
        int skip = (query.Page - 1) * query.PageSize;
        List<Project> pagedProjects = await projectsQuery
            .Skip(skip)
            .Take(query.PageSize)
            .Include(p => p.Members)
            .Include(p => p.Owner)
            .ToListAsync(cancellationToken);

        // Filter soft-deleted members manually after loading
        foreach (Project project in pagedProjects)
        {
            project.Members = [.. project.Members.Where(m => m.RecordStatus != RecordStatus.Deleted)];
        }
        
        var projectIds = pagedProjects.Select(p => p.Id).ToList();

        Dictionary<Guid, int> sectionCounts = await dbContext.Sections
            .Where(s => projectIds.Contains(s.ProjectId) && s.RecordStatus != RecordStatus.Deleted)
            .GroupBy(s => s.ProjectId)
            .Select(g => new { ProjectId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ProjectId, x => x.Count, cancellationToken);

        Dictionary<Guid, int> pageCounts = await dbContext.Pages
            .Where(p => projectIds.Contains(p.Section.ProjectId) && p.RecordStatus != RecordStatus.Deleted)
            .GroupBy(p => p.Section.ProjectId)
            .Select(g => new { ProjectId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ProjectId, x => x.Count, cancellationToken);

        // Mapping
        List<ProjectResponse>? projectResponses = mapper.Map<List<ProjectResponse>>(pagedProjects);
        
        foreach (ProjectResponse projectResponse in projectResponses)
        {
            projectResponse.SectionCount = sectionCounts.GetValueOrDefault(projectResponse.Id, 0);
            projectResponse.PageCount = pageCounts.GetValueOrDefault(projectResponse.Id, 0);
        }

        var pagedResult = new PagedResult<ProjectResponse>
        {
            Items = projectResponses,
            PageNumber = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return Result.Success(pagedResult);
    }
}
