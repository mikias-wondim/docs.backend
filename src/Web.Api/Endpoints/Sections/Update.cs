using Application.Abstractions.Messaging;
using Application.Features.Sections.Update;
using Domain.ProjectMembers;
using Domain.Sections;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Sections;

internal sealed class Update: IEndpoint
{
    private sealed record Request(
        string Name,
        string? Description,
        SectionVisibility Visibility,
        string? Password,
        List<ProjectRole>? AllowedRoles,
        List<Guid>? AllowedUserIds);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("sections/{sectionId:guid}", async (
                [FromRoute] Guid sectionId,
                [FromBody] Request request,
                ICommandHandler<UpdateSectionCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new UpdateSectionCommand(
                    sectionId,
                    request.Name,
                    request.Description,
                    request.Visibility,
                    request.Password,
                    request.AllowedRoles,
                    request.AllowedUserIds);

                Result result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            }).WithTags(Tags.Sections)
            .RequireAuthorization();   
    }
}
