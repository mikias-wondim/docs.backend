using Application.Abstractions.Messaging;
using Application.Features.Auth.Refresh;
using Application.Features.Users;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class Refresh: IEndpoint
{
    private sealed record Request(string RefreshToken);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/refresh", async (
                [FromBody] Request request,
                ICommandHandler<RefreshTokenCommand, UserLoginResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new RefreshTokenCommand(request.RefreshToken);

                Result<UserLoginResponse> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Auth);
    }
}
