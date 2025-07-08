using Application.Abstractions.Messaging;

namespace Application.Features.Media.Get;

public sealed record GetSectionAssetQuery(Guid SectionId, string RelativePath): IQuery<Stream>;
