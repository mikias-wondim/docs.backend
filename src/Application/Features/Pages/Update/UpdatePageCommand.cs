using Application.Abstractions.Messaging;

namespace Application.Features.Pages.Update;

public sealed record UpdatePageCommand(
    Guid PageId,
    string Title,
    string? ContentMd,
    List<string> Tags
) : ICommand;
