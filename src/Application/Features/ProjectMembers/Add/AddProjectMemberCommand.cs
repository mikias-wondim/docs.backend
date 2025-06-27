using Application.Abstractions.Messaging;
using Domain.ProjectMembers;

namespace Application.Features.ProjectMembers.Add;

public sealed record AddProjectMemberCommand(Guid ProjectId, Guid UserId, ProjectRole Role) : ICommand;
