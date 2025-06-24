using Application.Abstractions.Messaging;

namespace Application.Features.Projects.Delete;

public record DeleteProjectCommand(Guid ProjectId) : ICommand;
