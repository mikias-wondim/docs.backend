using Application.Abstractions.Messaging;
using Application.Features.Faqs;
using Application.Features.Faqs.GetByPageId;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Faqs;

internal sealed class GetByPageId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("pages/{pageId:guid}/faqs", async (
                [FromRoute] Guid pageId,
                IQueryHandler<GetFaqsByPageIdQuery, List<FaqResponse>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetFaqsByPageIdQuery(pageId);

                Result<List<FaqResponse>> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            }).WithTags(Tags.Faqs)
            .RequireAuthorization();
    }
}
