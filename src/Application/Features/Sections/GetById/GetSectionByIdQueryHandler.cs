using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Features.Pages;
using AutoMapper;
using Domain.Pages;
using Domain.ProjectMembers;
using Domain.Sections;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Sections.GetById;

internal sealed class GetSectionByIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IMapper mapper) : IQueryHandler<GetSectionByIdQuery, SectionResponse>
{
    public async Task<Result<SectionResponse>> Handle(GetSectionByIdQuery query, CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<SectionResponse>(UserErrors.Unauthorized);
        }

        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<SectionResponse>(UserErrors.NotFound(currentUserId));
        }

        Section? section = await context.Sections
            .Include(s => s.Pages)
            .Include(s => s.Project)
            .ThenInclude(p => p.Members)
            .Include(s => s.AllowedUsers)
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(s => s.Id == query.SectionId, cancellationToken);

        if (section is null)
        {
            return Result.Failure<SectionResponse>(SectionErrors.NotFound(query.SectionId));
        }

        ProjectRole? userRole = section.Project.OwnerId == currentUserId
            ? ProjectRole.Admin
            : section.Project.Members
                .FirstOrDefault(m => m.UserId == currentUserId)?.Role;

        bool isAccessible = section.IsAccessibleTo(
            user,
            userRole,
            query.Password
        );

        bool isOwner = section.Project.OwnerId == currentUserId;
        bool canWrite = section.Project.Members.Any(m => m.UserId == currentUserId && m.CanWrite());

        if (!isAccessible && !isOwner && !canWrite)
        {
            return Result.Failure<SectionResponse>(UserErrors.Forbidden);
        }

        SectionResponse response = mapper.Map<SectionResponse>(section);
        response.Pages = BuildPageTree(section.Pages, null, mapper);
        return Result.Success(response);
    }

    private static List<PageResponse> BuildPageTree(IEnumerable<Page> allPages, Guid? parentId, IMapper mapper)
    {
        IEnumerable<Page> enumerable = allPages as Page[] ?? [.. allPages];
        return
        [
            .. enumerable
                .Where(p => p.ParentPageId == parentId)
                .OrderBy(p => p.Order)
                .Select(p =>
                {
                    PageResponse? response = mapper.Map<PageResponse>(p);
                    response.Children = BuildPageTree(enumerable, p.Id, mapper);
                    return response;
                })
        ];
    }
}
