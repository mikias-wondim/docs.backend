using Application.Abstractions.Messaging;
using Application.Features.Users;
using Application.Features.Users.SearchByProjectId;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

internal sealed class SearchUsersByProjectId: IEndpoint
{
public void MapEndpoint(IEndpointRouteBuilder app)
{
    app.MapGet("users/search/member/{projectId:guid}", async (
            Guid projectId,
            [FromQuery]string query,
            IQueryHandler<SearchUsersByProjectIdQuery, List<UserSummaryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var searchQuery = new SearchUsersByProjectIdQuery(projectId, query);

            Result<List<UserSummaryResponse>> result = await handler.Handle(searchQuery, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();
}
}

