using Application.Abstractions.Messaging;

namespace Application.Features.Auth.VerifyEmail;

public sealed record VerifyEmailCommand(string Token): ICommand<bool>;
