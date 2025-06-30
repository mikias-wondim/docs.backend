using Application.Abstractions.Messaging;
using Domain.ProjectMembers;

namespace Application.Features.Invitations.Send;

public record SendInvitationCommand(Guid ProjectId, Guid UserId, ProjectRole Role): ICommand<bool>;
