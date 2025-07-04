using Application.Abstractions.Messaging;

namespace Application.Features.Pages.Delete;

public sealed record DeletePageCommand(Guid PageId): ICommand;
