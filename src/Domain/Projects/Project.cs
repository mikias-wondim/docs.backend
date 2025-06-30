using Domain.Invitations;
using Domain.ProjectMembers;
using Domain.Users;
using SharedKernel;

namespace Domain.Projects;

public sealed class Project : Entity
{
    public Guid OwnerId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public ProjectVisibility Visibility { get; private set; }
    public string? OverviewMd { get; private set; }

    public User Owner { get; set; }
    public List<ProjectMember> Members { get; set; }
    public List<Invitation> Invitations { get; set; }
    
    // Required by EF Core
    public Project()
    { }
    
    public Project(
        Guid id,
        Guid ownerId,
        string name,
        string? description,
        ProjectVisibility visibility,
        DateTime createdAt,
        string createdBy)
        : base(id)
    {
        OwnerId = ownerId;
        Name = name;
        OverviewMd = name;
        Visibility = visibility;
        Description = description;

        RegisterAudit(createdAt, createdBy);

        Raise(new ProjectCreatedDomainEvent(id));
    }
    
    public void Update(string newName, string? newDescription, ProjectVisibility visibility, string updatedBy, DateTime timestamp)
    {
        Name = newName;
        Description = newDescription;
        Visibility = visibility;
        
        UpdateAudit(timestamp, updatedBy);
        
        Raise(new ProjectUpdatedDomainEvent(Id));
    }
    
    public void UpdateOverview(string overviewMd, string updatedBy, DateTime timestamp)
    {
        OverviewMd = overviewMd;
        
        UpdateAudit(timestamp, updatedBy);
    }

    public void TransferOwnership(Guid newOwnerId, string updatedBy, DateTime timestamp)
    {
        OwnerId = newOwnerId;
        
        UpdateAudit(timestamp, updatedBy);
    }
}
