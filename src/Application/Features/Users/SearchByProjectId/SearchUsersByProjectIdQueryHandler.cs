using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Invitations;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.SearchByProjectId;

internal sealed class SearchUsersByProjectIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper) : IQueryHandler<SearchUsersByProjectIdQuery, List<UserSummaryResponse>>
{
    public async Task<Result<List<UserSummaryResponse>>> Handle(SearchUsersByProjectIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Query.Length < 3)
        {
            return Result.Success(new List<UserSummaryResponse>());
        }

        Guid ownerId = await context.Projects
            .Where(p => p.Id == query.ProjectId)
            .Select(p => p.OwnerId)
            .FirstOrDefaultAsync(cancellationToken);

        IQueryable<User> usersQuery = context.Users
            .AsNoTracking()
            .Where(u =>
                u.Id != ownerId &&
                !u.ProjectMembers.Any(pm =>
                    pm.ProjectId == query.ProjectId &&
                    pm.RecordStatus == RecordStatus.Active) &&
                !u.Invitations.Any(inv =>
                    inv.ProjectId == query.ProjectId &&
                    inv.Status == InvitationStatus.Pending)
            );

        string pattern = $"%{query.Query}%";

        usersQuery = usersQuery.Where(u =>
            EF.Functions.Like(u.DisplayName!, pattern) ||
            EF.Functions.Like(u.FirstName, pattern) ||
            EF.Functions.Like(u.LastName, pattern) ||
            EF.Functions.Like(u.Email, pattern));


        List<User> users = await usersQuery.ToListAsync(cancellationToken);
        List<UserSummaryResponse> result = mapper.Map<List<UserSummaryResponse>>(users);

        return Result.Success(result);
    }
}
