using Application.Abstractions.Messaging;

namespace Application.Features.Faqs.Move;

public sealed record MoveFaqCommand(
    Guid FaqId,
    int NewIndex
) : ICommand;
