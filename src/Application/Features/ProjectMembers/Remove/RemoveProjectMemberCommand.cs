using Application.Abstractions.Messaging;

namespace Application.Features.ProjectMembers.Remove;

public record RemoveProjectMemberCommand(Guid ProjectId, Guid UserId) : ICommand;
