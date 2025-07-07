using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Pages;
using Domain.ProjectMembers;
using Domain.Sections;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Pages.GetBySectionId;

internal sealed class GetPagesBySectionIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IMapper mapper
) : IQueryHandler<GetPagesBySectionIdQuery, List<PageSummaryResponse>>
{
    public async Task<Result<List<PageSummaryResponse>>> Handle(GetPagesBySectionIdQuery query,
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
            return Result.Failure<List<PageSummaryResponse>>(SectionErrors.NotFound(query.SectionId));
        }

        if (section.Visibility != SectionVisibility.Public)
        {
            Guid? currentUserId;
            try { currentUserId = userContext.UserId; }
            catch { return Result.Failure<List<PageSummaryResponse>>(UserErrors.Unauthorized); }

            User? user = await context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

            if (user is null)
            {
                return Result.Failure<List<PageSummaryResponse>>(UserErrors.NotFound(currentUserId.Value));
            }

            ProjectRole? role = section.Project.Members
                .FirstOrDefault(m => m.UserId == currentUserId)?.Role;

            if (!section.IsAccessibleTo(user, role))
            {
                return Result.Failure<List<PageSummaryResponse>>(UserErrors.Forbidden);
            }
        }

        IQueryable<Page> pagesQuery = context.Pages
            .AsNoTracking()
            .Where(p => p.SectionId == query.SectionId);

        if (!string.IsNullOrWhiteSpace(query.Tag))
        {
            string loweredTag = query.Tag.ToLower(System.Globalization.CultureInfo.CurrentCulture);

            pagesQuery = context.FromSqlInterpolated<Page>(
                $"""
                 SELECT p.* FROM Pages p
                 WHERE p.SectionId = {query.SectionId}
                   AND ';' + p.Tags + ';' LIKE CONCAT('%;', {loweredTag}, ';%')
                 """);
        }
        else if (!string.IsNullOrWhiteSpace(query.Query))
        {
            string loweredQuery = query.Query.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            pagesQuery = pagesQuery.Where(p =>
                EF.Functions.Like(p.Title, $"%{loweredQuery}%") ||
                EF.Functions.Like(p.ContentMd, $"%# {loweredQuery}%") ||
                EF.Functions.Like(p.ContentMd, $"%## {loweredQuery}%"));
        }

        List<Page> pages = await pagesQuery
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(cancellationToken);

        return Result.Success(mapper.Map<List<PageSummaryResponse>>(pages));
    }
}
