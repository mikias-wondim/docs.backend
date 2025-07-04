using Application.Abstractions.Messaging;

namespace Application.Features.Faqs.Create;

public sealed record CreateFaqCommand(
    Guid PageId,
    string Question,
    string Answer
) : ICommand<Guid>;
