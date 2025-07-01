using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.Get;

internal sealed class GetUsersQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
) : IQueryHandler<GetUsersQuery, PagedResult<UserSummaryResponse>>
{
    public async Task<Result<PagedResult<UserSummaryResponse>>> Handle(GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<User> usersQuery = context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string pattern = $"%{query.Search.Trim()}%";

            usersQuery = usersQuery.Where(u =>
                u.DisplayName != null && EF.Functions.Like(u.DisplayName, pattern) ||
                EF.Functions.Like(u.FirstName, pattern) ||
                EF.Functions.Like(u.LastName, pattern) ||
                EF.Functions.Like(u.Email, pattern));
        }

        // Sorting
        usersQuery = (query.SortBy?.ToLower(System.Globalization.CultureInfo.CurrentCulture),
                query.SortOrder?.ToLower(System.Globalization.CultureInfo.CurrentCulture)) switch
            {
                ("email", "asc") => usersQuery.OrderBy(u => u.Email),
                ("email", "desc") => usersQuery.OrderByDescending(u => u.Email),
                ("firstname", "asc") => usersQuery.OrderBy(u => u.FirstName),
                ("firstname", "desc") => usersQuery.OrderByDescending(u => u.FirstName),
                ("lastname", "asc") => usersQuery.OrderBy(u => u.LastName),
                ("lastname", "desc") => usersQuery.OrderByDescending(u => u.LastName),
                ("createdat", "asc") => usersQuery.OrderBy(u => u.CreatedAt),
                _ => usersQuery.OrderByDescending(u => u.CreatedAt)
            };

        // Total count before pagination
        int totalCount = await usersQuery.CountAsync(cancellationToken);

        int skip = (query.Page - 1) * query.PageSize;

        List<User> users = await usersQuery
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        List<UserSummaryResponse> userResponses = mapper.Map<List<UserSummaryResponse>>(users);

        var pagedResult = new PagedResult<UserSummaryResponse>
        {
            Items = userResponses,
            PageNumber = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return Result.Success(pagedResult);
    }
}
