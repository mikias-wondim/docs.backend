using Application.Abstractions.Messaging;

namespace Application.Features.Pages.Move;

public sealed record MovePageCommand(
    Guid PageId,
    Guid? NewParentPageId,
    int NewIndex
) : ICommand;
