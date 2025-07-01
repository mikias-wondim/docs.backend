using Application.Abstractions.Messaging;
using Application.Features.Users;
using Application.Features.Users.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

internal sealed class Search: IEndpoint
{
    private sealed record QueryParams(
        string Query,
        string? SortBy = "firstName",
        string? SortOrder = "desc",
        int Page = 1,
        int PageSize = 20);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/search", async (
                [AsParameters] QueryParams queryParams,
                IQueryHandler<GetUsersQuery, PagedResult<UserSummaryResponse>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetUsersQuery(
                    queryParams.Query,
                    queryParams.SortBy,
                    queryParams.SortOrder,
                    queryParams.Page,
                    queryParams.PageSize
                );

                Result<PagedResult<UserSummaryResponse>> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Users)
            .RequireAuthorization();
    }
}
