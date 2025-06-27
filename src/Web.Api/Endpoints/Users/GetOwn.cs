using Application.Abstractions.Messaging;
using Application.Features.Users;
using Application.Features.Users.GetOwn;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

internal sealed class GetOwn : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/me", async (
                IQueryHandler<GetOwnQuery, UserResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetOwnQuery();

                Result<UserResponse> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .HasPermission(Permissions.UsersAccess)
            .WithTags(Tags.Users);
    }
}
