using Application.Abstractions.Messaging;
using Application.Features.Invitations.Respond;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Invitations;

public class Respond : IEndpoint
{
    private sealed record Request(bool Accept);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("projects/invitations/{token}/respond", async (
                [FromRoute] string token,
                [FromBody] Request request,
                ICommandHandler<RespondToInvitationCommand, bool> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new RespondToInvitationCommand(token, request.Accept);
                Result<bool> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Invitations)
            .RequireAuthorization();
    }
}
