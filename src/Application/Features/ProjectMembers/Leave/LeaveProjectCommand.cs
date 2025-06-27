using Application.Abstractions.Messaging;

namespace Application.Features.ProjectMembers.Leave;

public sealed record LeaveProjectCommand(Guid ProjectId) : ICommand;
