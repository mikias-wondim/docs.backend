using Application.Abstractions.Messaging;

namespace Application.Features.Faqs.Delete;

public sealed record DeleteFaqCommand(Guid FaqId): ICommand;
