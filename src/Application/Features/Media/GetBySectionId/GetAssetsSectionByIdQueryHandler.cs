using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Media;
using Domain.ProjectMembers;
using Domain.Sections;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Media.GetBySectionId;

internal sealed class GetAssetsSectionByIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IMapper mapper
) : IQueryHandler<GetAssetsSectionByIdQuery, PagedResult<MediaAssetResponse>>
{
    public async Task<Result<PagedResult<MediaAssetResponse>>> Handle(
        GetAssetsSectionByIdQuery query,
        CancellationToken cancellationToken)
    {
        Section? section = await context.Sections
            .Include(s => s.Project)
            .ThenInclude(p => p.Members)
            .Include(s => s.AllowedUsers)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == query.SectionId && s.RecordStatus != RecordStatus.Deleted,
                cancellationToken);

        if (section is null)
        {
            return Result.Failure<PagedResult<MediaAssetResponse>>(SectionErrors.NotFound(query.SectionId));
        }

        if (section.Visibility != SectionVisibility.Public)
        {
            Guid userId = userContext.UserId;
            User? user = await context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
            {
                return Result.Failure<PagedResult<MediaAssetResponse>>(UserErrors.NotFound(userId));
            }

            ProjectRole? role = section.Project.Members.FirstOrDefault(m => m.UserId == userId)?.Role;

            if (!section.IsAccessibleTo(user, role))
            {
                return Result.Failure<PagedResult<MediaAssetResponse>>(UserErrors.Forbidden);
            }
        }
        
        IQueryable<MediaAsset> assetsQuery = context.MediaAssets
            .AsNoTracking()
            .Where(a => a.SectionId == query.SectionId);
       
        assetsQuery = (query.SortBy?.ToLower(System.Globalization.CultureInfo.CurrentCulture),
                query.SortOrder?.ToLower(System.Globalization.CultureInfo.CurrentCulture)) switch
            {
                ("name", "asc") => assetsQuery.OrderBy(p => p.Name),
                ("name", "desc") => assetsQuery.OrderByDescending(p => p.Name),
                ("updatedat", "asc") => assetsQuery.OrderBy(p => p.UpdatedAt),
                ("updatedat", "desc") => assetsQuery.OrderByDescending(p => p.UpdatedAt),
                ("createdat", "asc") => assetsQuery.OrderBy(p => p.CreatedAt),
                _ => assetsQuery.OrderByDescending(p => p.UpdatedAt)
            };
        
        int totalCount = await assetsQuery.CountAsync(cancellationToken);

        int skip = (query.Page - 1) * query.PageSize;
        List<MediaAsset> assets = await context.MediaAssets
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        List<MediaAssetResponse> responses = [.. assets.Select(mapper.Map<MediaAssetResponse>)];
        
        var result = new PagedResult<MediaAssetResponse>
        {
            Items = responses,
            PageNumber = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
        return Result.Success(result);
    }
}
