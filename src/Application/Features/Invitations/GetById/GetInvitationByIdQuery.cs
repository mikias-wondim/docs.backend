using Application.Abstractions.Messaging;

namespace Application.Features.Invitations.GetById;

public sealed record GetInvitationByIdQuery(Guid Id): IQuery<InvitationResponse>;
