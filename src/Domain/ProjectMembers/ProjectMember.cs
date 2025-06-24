using Domain.Projects;
using Domain.Users;
using SharedKernel;

namespace Domain.ProjectMembers;

public sealed class ProjectMember: Entity
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public ProjectRole Role { get; private set; }

    // Navigation Properties
    public Project Project { get; set; }
    public User User { get; set; }
    
    // EF Core Constructor
    private ProjectMember() { }
    
    public ProjectMember(Guid id, Guid projectId, Guid userId, ProjectRole role)
        : base(id)
    {
        ProjectId = projectId;
        UserId = userId;
        Role = role;
    }

    public void ChangeRole(ProjectRole newRole)
    {
        if (Role == ProjectRole.Owner)
        {
            throw new InvalidOperationException("Owner role cannot be changed.");
        }

        Role = newRole;
    }

    public bool IsAdmin() => Role == ProjectRole.Admin;
    public bool CanWrite() => Role is ProjectRole.Write or ProjectRole.Admin or ProjectRole.Owner;
}
