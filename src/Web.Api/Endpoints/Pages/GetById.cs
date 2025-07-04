using Application.Abstractions.Messaging;
using Application.Features.Pages;
using Application.Features.Pages.GetById;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Pages;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("pages/{pageId:guid}", async (
                [FromRoute] Guid pageId,
                [FromQuery] string? password,
                IQueryHandler<GetPageByIdQuery, PageResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetPageByIdQuery(
                    pageId, password);

                Result<PageResponse> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            }).WithTags(Tags.Pages)
            .RequireAuthorization();
    }
}
