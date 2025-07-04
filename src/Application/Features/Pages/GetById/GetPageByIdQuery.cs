using Application.Abstractions.Messaging;

namespace Application.Features.Pages.GetById;

public sealed record GetPageByIdQuery(
    Guid PageId,
    string? SectionPassword
) : IQuery<PageResponse>;
