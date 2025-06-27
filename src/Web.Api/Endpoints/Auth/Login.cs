using Application.Abstractions.Messaging;
using Application.Features.Auth;
using Application.Features.Auth.Login;
using Application.Features.Users;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class Login : IEndpoint
{
    private sealed record Request(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (
            Request request,
            ICommandHandler<LoginCommand, AuthLoginResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Email, request.Password);

            Result<AuthLoginResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auth);
    }
}
