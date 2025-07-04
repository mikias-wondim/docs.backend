using Application.Abstractions.Messaging;
using Application.Features.Sections;
using Application.Features.Sections.GetById;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Sections;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("sections/{sectionId:guid}", async (
                [FromRoute]Guid sectionId,
                [FromQuery]string? password,
                IQueryHandler<GetSectionByIdQuery, SectionResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetSectionByIdQuery(
                    sectionId, password);
                
                Result<SectionResponse> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            }).WithTags(Tags.Sections)
            .RequireAuthorization();
    }
}
