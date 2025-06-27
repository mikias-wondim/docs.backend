using Application.Abstractions.Messaging;
using Application.Features.ProjectMembers.Add;
using Domain.ProjectMembers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.ProjectMembers;

internal sealed class Add : IEndpoint
{
    private sealed record Request(
        Guid UserId,
        ProjectRole Role);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/projects/{projectId:guid}/member", async (
                Guid projectId,
                [FromBody] Request request,
                ICommandHandler<AddProjectMemberCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new AddProjectMemberCommand(
                    projectId,
                    request.UserId,
                    request.Role);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Created, CustomResults.Problem);
            })
            .WithTags(Tags.ProjectMembers)
            .RequireAuthorization();
    }
}
