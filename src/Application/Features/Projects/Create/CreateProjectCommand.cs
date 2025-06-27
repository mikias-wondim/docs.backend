using Application.Abstractions.Messaging;
using Domain.Projects;

namespace Application.Features.Projects.Create;

public sealed record CreateProjectCommand(
    string Name,
    string? Description,
    ProjectVisibility Visibility
) : ICommand<Guid>;
