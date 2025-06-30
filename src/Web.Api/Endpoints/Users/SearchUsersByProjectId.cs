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
    private sealed record Request(string Query);
public void MapEndpoint(IEndpointRouteBuilder app)
{
    app.MapGet("users/search/member/{projectId:guid}", async (
            Guid projectId,
            [FromBody]Request request,
            IQueryHandler<SearchUsersByProjectIdQuery, List<UserSummaryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new SearchUsersByProjectIdQuery(projectId, request.Query);

            Result<List<UserSummaryResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.UsersAccess)
        .WithTags(Tags.Users);
}
}

