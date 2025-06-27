using Application.Abstractions.Messaging;
using Application.Features.Users.Register;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

internal sealed class Register : IEndpoint
{
    private sealed record Request(string Email, string FirstName, string LastName, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (
            [FromBody]Request request,
            ICommandHandler<RegisterUserCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Created, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
