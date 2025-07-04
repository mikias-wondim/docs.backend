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

namespace Application.Features.Pages.GetById;

internal sealed class GetPageByIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IMapper mapper
) : IQueryHandler<GetPageByIdQuery, PageResponse>
{
    public async Task<Result<PageResponse>> Handle(GetPageByIdQuery query, CancellationToken cancellationToken)
    {
        Page? page = await context.Pages
            .Include(p => p.ParentPage)
            .Include(p => p.Section)
                .ThenInclude(s => s.Project)
                    .ThenInclude(p => p.Members)
            .Include(p => p.Section)
            .ThenInclude(s => s.Pages)
            .Include(p => p.Section.AllowedUsers)
            .FirstOrDefaultAsync(p => p.Id == query.PageId, cancellationToken);

        if (page is null)
        {
            return Result.Failure<PageResponse>(PageErrors.NotFound(query.PageId));
        }

        Section section = page.Section;

        if (section.Visibility == SectionVisibility.Public)
        {
            PageResponse? publicResponse = mapper.Map<PageResponse>(page);
            return Result.Success(publicResponse);
        }
        
        Guid? currentUserId;
        try { currentUserId = userContext.UserId; } catch { currentUserId = null; }

        if (currentUserId.HasValue)
        {
            User? user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

            if (user is null)
            {
                return Result.Failure<PageResponse>(UserErrors.NotFound((Guid)currentUserId));
            }
            
            ProjectRole? role = section.Project.Members
                .FirstOrDefault(m => m.UserId == currentUserId)?.Role;

            bool hasAccess = section.Visibility switch
            {
                SectionVisibility.ProtectedByRole => section.AllowedRoles?.Contains(role ?? default) == true,
                SectionVisibility.ProtectedByUser => section.AllowedUsers.Any(a => a.UserId == currentUserId),
                SectionVisibility.ProtectedWithPassword => query.SectionPassword is not null
                                                           && section.Password == query.SectionPassword,
                _ => false
            };

            bool isOwner = section.Project.OwnerId == currentUserId;
            bool canWrite = section.Project.Members.Any(m => m.UserId == currentUserId && m.CanWrite());
            hasAccess |= isOwner || canWrite;
            
            return !hasAccess ? Result.Failure<PageResponse>(PageErrors.Forbidden) : Result.Success(mapper.Map<PageResponse>(page));
        }
        
        if (section.Visibility == SectionVisibility.ProtectedWithPassword &&
            query.SectionPassword is not null &&
            section.Password == query.SectionPassword)
        {
            return Result.Success(mapper.Map<PageResponse>(page));
        }

        return Result.Failure<PageResponse>(PageErrors.Forbidden);
    }
}
