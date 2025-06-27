using Application.Abstractions.Messaging;
using Application.Features.Users.UpdateProfile;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

public class UpdateProfile : IEndpoint
{
    private sealed class Request
    {
        [FromForm] public string FirstName { get; init; } = null!;
        [FromForm] public string LastName { get; init; } = null!;
        [FromForm] public string? DisplayName { get; init; }
        [FromForm] public IFormFile? Avatar { get; init; }
        [FromForm] public string? Bio { get; init; }
    }


    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/users/me", async (
                [FromForm] Request request,
                ICommandHandler<UpdateProfileCommand, Guid> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateProfileCommand(
                    request.FirstName,
                    request.LastName,
                    request.DisplayName,
                    request.Avatar,
                    request.Bio);

                Result<Guid> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .Accepts<IFormFile>("multipart/form-data")
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithTags(Tags.Users);
    }
}
