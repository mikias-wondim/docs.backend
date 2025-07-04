using Application.Features.Invitations;
using Application.Features.ProjectMembers;
using Application.Features.Sections;
using Application.Features.Users;
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
    
    public UserSummaryResponse Owner { get; set; }
    public List<ProjectMemberResponse> Members { get; set; }
    public List<InvitationResponse> Invitations { get; set; }
    public List<SectionResponse> Sections { get; set; }

    public int? SectionCount { get; set; }
    public int? PageCount { get; set; }
}
