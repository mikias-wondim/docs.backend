using Application.Abstractions.Messaging;

namespace Application.Features.Faqs.Update;

public sealed record UpdateFaqCommand(
    Guid FaqId,
    string Question,
    string Answer
) : ICommand;
