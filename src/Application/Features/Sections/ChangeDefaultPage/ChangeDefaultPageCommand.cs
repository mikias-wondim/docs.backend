using Application.Abstractions.Messaging;

namespace Application.Features.Sections.ChangeDefaultPage;

public sealed record ChangeDefaultPageCommand(Guid SectionId, Guid PageId): ICommand;
