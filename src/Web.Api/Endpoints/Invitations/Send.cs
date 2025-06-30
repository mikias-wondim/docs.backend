using Application.Abstractions.Messaging;
using Application.Features.Invitations.Send;
using Domain.ProjectMembers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Invitations;

internal sealed class Send: IEndpoint
{    
    private sealed record Request(
        Guid UserId,
        ProjectRole Role);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("projects/{projectId:guid}/invitation", async (
            [FromRoute]Guid projectId, 
            [FromBody]Request request,
            ICommandHandler<SendInvitationCommand, bool> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SendInvitationCommand(projectId, request.UserId,
                request.Role);

            Result<bool> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Invitations)
        .RequireAuthorization();
    }
}
