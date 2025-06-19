using Application.Abstractions.Messaging;
using Domain.Projects;

namespace Application.Features.Projects.Update;

public class UpdateProjectCommand: ICommand<Guid>
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public ProjectVisibility Visibility { get; set; }
}
