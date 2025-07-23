
using Application.Abstractions.Messaging;

namespace Application.Features.Feedbacks.Create;

public sealed record CreateFeedbackCommand(Guid PageId, ushort Rating, string? Comment): ICommand;
