using Application.Features.Projects;
using Domain.ProjectMembers;
using Domain.Sections;
using SharedKernel;

namespace Application.Features.Sections;

public sealed class SectionResponse: EntityResponse
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public SectionVisibility Visibility { get; set; }
    public string? Password { get; set; }
    public int Order { get; set; }

    public List<ProjectRole>? AllowedRoles { get; set; }
    public List<SectionUserAccessResponse> AllowedUsers { get; set; } = [];
    public ProjectSummaryResponse Project { get; set; }
}
