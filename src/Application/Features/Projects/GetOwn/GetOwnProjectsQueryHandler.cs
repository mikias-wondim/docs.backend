using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Projects.GetOwn;

internal sealed class GetOwnProjectsQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IMapper mapper
) : IQueryHandler<GetOwnProjectsQuery, PagedResult<ProjectResponse>>
{
    public async Task<Result<PagedResult<ProjectResponse>>> Handle(GetOwnProjectsQuery query,
        CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<PagedResult<ProjectResponse>>(UserErrors.Unauthorized);
        }

        IQueryable<Project> projectsQuery = context.Projects
            .AsNoTracking()
            .Where(p => p.RecordStatus != RecordStatus.Deleted &&
                        (p.OwnerId == currentUserId || p.Members.Any(m => m.UserId == currentUserId)));

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

        // Mapping
        List<ProjectResponse>? projectResponses = mapper.Map<List<ProjectResponse>>(pagedProjects);

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
