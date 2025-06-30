using Application.Features.Invitations.GetById;
using Application.Abstractions.Messaging;
using Application.Features.Invitations;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Invitations;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("projects/invitations/{token:guid}", async (
                [FromRoute] Guid token,
                IQueryHandler<GetInvitationByIdQuery, InvitationResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetInvitationByIdQuery(token);
                Result<InvitationResponse> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Invitations);
    }
}
