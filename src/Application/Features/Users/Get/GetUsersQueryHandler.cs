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
) : IQueryHandler<GetUsersQuery, List<UserResponse>>
{
    public async Task<Result<List<UserResponse>>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        IQueryable<User> usersQuery = context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string search = query.Search.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            usersQuery = usersQuery.Where(u =>
                u.DisplayName != null && u.DisplayName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                u.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                u.LastName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                u.Email.Contains(search, StringComparison.CurrentCultureIgnoreCase));
        }

        if (query.EmailVerified.HasValue)
        {
            usersQuery = usersQuery.Where(u => u.EmailVerified == query.EmailVerified);
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

        int skip = (query.Page - 1) * query.PageSize;

        List<User> users = await usersQuery
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        List<UserResponse> result = mapper.Map<List<UserResponse>>(users);

        return Result.Success(result);
    }
}
