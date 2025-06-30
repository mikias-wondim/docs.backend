using Application.Abstractions.Messaging;

namespace Application.Features.Invitations.Respond;

public sealed record RespondToInvitationCommand(string Token, bool Accept) : ICommand<bool>;

