using Application.Abstractions.Messaging;
using Application.Features.ProjectMembers.Remove;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.ProjectMembers;

internal sealed class Remove : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/projects/{projectId:guid}/members/{userId:guid}",
                async (
                    Guid projectId,
                    Guid userId,
                    ICommandHandler<RemoveProjectMemberCommand> handler,
                    CancellationToken cancellationToken) =>
                {
                    var command = new RemoveProjectMemberCommand(projectId,
                        userId);

                    Result result = await handler.Handle(command,
                        cancellationToken);

                    return result.Match(Results.NoContent,
                        CustomResults.Problem);
                })
            .WithTags(Tags.ProjectMembers)
            .RequireAuthorization();
    }
}
