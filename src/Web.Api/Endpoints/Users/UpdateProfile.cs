using Application.Abstractions.Messaging;
using Application.Features.Users.UpdateProfile;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

public class UpdateProfile : IEndpoint
{
    private sealed record Request(
        string FirstName,
        string LastName,
        string? DisplayName,
        IFormFile? Avatar,
        string? Bio);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/{userId:guid}/update-profile", async (
                [FromRoute] Guid userId,
                [FromBody] Request request,
                ICommandHandler<UpdateProfileCommand, Guid> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateProfileCommand(
                    userId,
                    request.FirstName,
                    request.LastName,
                    request.DisplayName,
                    request.Avatar,
                    request.Bio);

                Result<Guid> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Users);
    }
}
