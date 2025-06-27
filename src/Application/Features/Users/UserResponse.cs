using Application.Features.ProjectMembers;
using Application.Features.Projects;
using SharedKernel;

namespace Application.Features.Users;

public sealed class UserResponse : EntityResponse
{
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }

    public string? DisplayName { get; init; }
    public string? AvatarUrl { get; init; }
    public string? Bio { get; init; }

    public bool EmailVerified { get;  init; }
    
    public List<ProjectResponse> Projects { get; private set; } = [];
    public List<ProjectMemberResponse> ProjectMembers { get; private set; } = [];
}
