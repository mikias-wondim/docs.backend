using Application.Abstractions.Messaging;

namespace Application.Features.Users.Get;

public sealed record GetUsersQuery(
    string? Search = null,
    bool? EmailVerified = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc",
    int Page = 1,
    int PageSize = 20
) : IQuery<List<UserResponse>>;
