using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
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
        IQueryable<User> usersQuery = context.Users.AsNoTracking()
            .Where(u => u.Projects.All(p => p.Id != query.ProjectId) &&
                        u.ProjectMembers.All(pm => pm.ProjectId != query.ProjectId));

        string search = query.Query.ToLower(System.Globalization.CultureInfo.CurrentCulture);
        if (search.Length < 3)
        {
            return Result.Success(new List<UserSummaryResponse>());
        }

        usersQuery = usersQuery.Where(u =>
            u.DisplayName != null && u.DisplayName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
            u.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
            u.LastName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
            u.Email.Contains(search, StringComparison.CurrentCultureIgnoreCase));

        List<User> users = await usersQuery
            .ToListAsync(cancellationToken);

        List<UserSummaryResponse> result = mapper.Map<List<UserSummaryResponse>>(users);

        return Result.Success(result);
    }
}
