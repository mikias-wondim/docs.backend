using Application.Abstractions.Messaging;
using Application.Features.ProjectMembers.Leave;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.ProjectMembers;

internal sealed class Leave : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/projects/{projectId:guid}/members/me",
                async (
                    Guid projectId,
                    ICommandHandler<LeaveProjectCommand> handler,
                    CancellationToken cancellationToken) =>
                {
                    var command = new LeaveProjectCommand(projectId);

                    Result result = await handler.Handle(command,
                        cancellationToken);

                    return result.Match(Results.NoContent,
                        CustomResults.Problem);
                })
            .WithTags(Tags.ProjectMembers)
            .RequireAuthorization();
    }
}
