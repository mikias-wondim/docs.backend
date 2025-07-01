using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Users.Get;

public sealed record GetUsersQuery(
    string? Search = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc",
    int Page = 1,
    int PageSize = 20
) : IQuery<PagedResult<UserSummaryResponse>>;
