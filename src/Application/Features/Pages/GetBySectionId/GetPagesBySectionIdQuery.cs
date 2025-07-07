using Application.Abstractions.Messaging;

namespace Application.Features.Pages.GetBySectionId;

public sealed record GetPagesBySectionIdQuery(Guid SectionId, string? Query, string? Tag):IQuery<List<PageSummaryResponse>>;
