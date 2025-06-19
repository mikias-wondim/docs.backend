using Application.Abstractions.Messaging;
using Domain.Projects;

namespace Application.Features.Projects.Create;

public sealed class CreateProjectCommand: ICommand<Guid>
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public ProjectVisibility Visibility { get; set; }
}
