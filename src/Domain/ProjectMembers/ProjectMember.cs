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
    public ProjectMember() { }
    
    public ProjectMember(Guid id, Guid projectId, Guid userId, ProjectRole role,  string createdBy, DateTime createdAt)
        : base(id)
    {
        ProjectId = projectId;
        UserId = userId;
        Role = role;
        
        RegisterAudit(createdAt, createdBy);
    }

    public void ChangeRole(ProjectRole newRole, string updatedBy, DateTime timestamp)
    {
        Role = newRole;
        
        UpdateAudit(timestamp, updatedBy);
    }

    public bool IsAdmin() => Role == ProjectRole.Admin;
    public bool CanWrite() => Role is ProjectRole.Write or ProjectRole.Admin;
}
