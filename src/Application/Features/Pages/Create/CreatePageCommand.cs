using Application.Abstractions.Messaging;

namespace Application.Features.Pages.Create;

public sealed record CreatePageCommand(
    Guid SectionId,
    string Title,
    Guid? ParentPageId,
    string? ContentMd,
    List<string> Tags
) : ICommand<Guid>;
