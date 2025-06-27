using Application.Abstractions.Messaging;
using Domain.ProjectMembers;

namespace Application.Features.ProjectMembers.UpdateRole;

public record UpdateProjectMemberRoleCommand(Guid ProjectId, Guid UserId, ProjectRole NewRole) : ICommand;
