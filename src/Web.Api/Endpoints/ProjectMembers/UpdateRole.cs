using Application.Abstractions.Messaging;
using Application.Features.ProjectMembers.UpdateRole;
using Domain.ProjectMembers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.ProjectMembers;

internal sealed class UpdateRole : IEndpoint
{
    private sealed record Request(ProjectRole NewRole);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/projects/{projectId:guid}/members/{userId:guid}",
                async (
                    Guid projectId,
                    Guid userId,
                    [FromBody] Request request,
                    ICommandHandler<UpdateProjectMemberRoleCommand> handler,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateProjectMemberRoleCommand(
                        projectId,
                        userId,
                        request.NewRole);

                    Result result = await handler.Handle(command,
                        cancellationToken);

                    return result.Match(Results.NoContent,
                        CustomResults.Problem);
                })
            .WithTags(Tags.ProjectMembers)
            .RequireAuthorization();
    }
}
