using Application.Abstractions.Messaging;

namespace Application.Features.Sections.Delete;

public sealed record DeleteSectionCommand(Guid SectionId): ICommand;
