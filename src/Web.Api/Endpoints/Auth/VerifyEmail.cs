using Application.Abstractions.Messaging;
using Application.Features.Auth.VerifyEmail;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class VerifyEmail: IEndpoint
{
    public const string EmailVerification = "VerifyEmail";
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/verify-email", async (
                [FromQuery] string token,
                ICommandHandler<VerifyEmailCommand, bool> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new VerifyEmailCommand(token);

                Result<bool> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithName(EmailVerification)
            .WithTags(Tags.Auth);
    }
}
