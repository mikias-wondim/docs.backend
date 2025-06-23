using Application.Abstractions.Messaging;
using Application.Features.Users.ChangePassword;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

public class ChangePassword : IEndpoint
{
    private sealed record Request(
        string CurrentPassword,
        string NewPassword);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/{userId:guid}/change-password", async (
                [FromRoute] Guid userId,
                [FromBody] Request request,
                ICommandHandler<ChangePasswordCommand, bool> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new ChangePasswordCommand(
                    userId,
                    request.CurrentPassword,
                    request.NewPassword);

                Result<bool> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Users);
    }
}
