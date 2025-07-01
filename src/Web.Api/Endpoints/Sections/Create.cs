using Application.Abstractions.Messaging;
using Application.Features.Sections.Create;
using Domain.ProjectMembers;
using Domain.Sections;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Sections;

internal sealed class Create : IEndpoint
{
    private sealed record Request(
        string Name,
        string? Description,
        SectionVisibility Visibility,
        string? Password,
        List<ProjectRole>? AllowedRoles,
        List<Guid>? AllowedUserIds,
        int Order);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/projects/{projectId:guid}/sections", async (
                [FromRoute] Guid projectId,
                [FromBody] Request request,
                ICommandHandler<CreateSectionCommand, Guid> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new CreateSectionCommand(
                    projectId,
                    request.Name,
                    request.Description,
                    request.Visibility,
                    request.Password,
                    request.AllowedRoles,
                    request.AllowedUserIds,
                    request.Order);

                Result<Guid> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            }).WithTags(Tags.Sections)
            .RequireAuthorization();
    }
}
