using Domain.Projects;
using SharedKernel;

namespace Application.Features.Projects;

public sealed class ProjectResponse: EntityResponse
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public ProjectVisibility Visibility { get; set; }
    public string? OverviewMd { get; set; }
}
