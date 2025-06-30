using Application.Features.Projects;
using Application.Features.Users;
using Domain.ProjectMembers;
using SharedKernel;

namespace Application.Features.ProjectMembers;

public sealed class ProjectMemberResponse: EntityResponse
{
    public Guid ProjectId { get; init; }
    public Guid UserId { get; init; }
    public ProjectRole Role { get; init; }
    
    public ProjectSummaryResponse Project { get; set; }
    public UserSummaryResponse User { get; set; }
}
