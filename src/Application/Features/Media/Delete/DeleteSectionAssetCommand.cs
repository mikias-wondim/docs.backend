using Application.Abstractions.Messaging;

namespace Application.Features.Media.Delete;

public sealed record DeleteSectionAssetCommand(Guid SectionId, string RelativePath) : ICommand;
