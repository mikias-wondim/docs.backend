using Application.Abstractions.Messaging;

namespace Application.Features.Sections.GetById;

public sealed record GetSectionByIdQuery(Guid SectionId, string? Password): IQuery<SectionResponse>;
