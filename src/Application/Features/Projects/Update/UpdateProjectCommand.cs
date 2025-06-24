using Application.Abstractions.Messaging;
using Domain.Projects;

namespace Application.Features.Projects.Update;

public sealed record UpdateProjectCommand(
    Guid ProjectId,
    string Name,
    string? Description,
    ProjectVisibility Visibility
) : ICommand<Guid>;
