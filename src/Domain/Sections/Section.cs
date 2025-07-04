using Domain.Pages;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Users;
using SharedKernel;

namespace Domain.Sections;

public sealed class Section : Entity
{
    public Guid ProjectId { get; private set; }
    public Guid DefaultPageId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public int Order { get; private set; }

    public SectionVisibility Visibility { get; private set; }
    public string? Password { get; private set; }
    public List<ProjectRole>? AllowedRoles { get; private set; }
    public List<SectionUserAccess> AllowedUsers { get; private set; } = [];
    
    public Project Project { get; set; }
    public List<Page> Pages { get; set; }

    // Required by EF
    public Section() { }

    public Section(
        Guid id,
        Guid projectId,
        string name,
        string? description,
        SectionVisibility visibility,
        int order,
        DateTime createdAt,
        string createdBy,
        string? password = null,
        List<ProjectRole>? allowedRoles = null,
        List<Guid>? allowedUserIds = null
    ) : base(id)
    {
        ProjectId = projectId;
        Name = name;
        Description = description;
        Visibility = visibility;
        Password = password;
        AllowedRoles = allowedRoles;
        Order = order;

        if (allowedUserIds is not null)
        {
            AllowedUsers =
            [
                .. allowedUserIds.Select(uid => new SectionUserAccess
                (
                    Guid.NewGuid(),
                    id,
                    uid,
                    createdBy,
                    createdAt
                ))
            ];
        }

        RegisterAudit(createdAt, createdBy);
    }

    public void Update(
        string name,
        string? description,
        SectionVisibility visibility,
        string? password,
        List<ProjectRole>? allowedRoles,
        List<Guid> allowedUserIds,
        string updatedBy,
        DateTime timestamp)
    {
        Name = name;
        Description = description;
        Visibility = visibility;
        Password = password;
        AllowedRoles = allowedRoles;

        var currentIds = AllowedUsers.Select(a => a.UserId).ToList();

        AllowedUsers.RemoveAll(a => !allowedUserIds.Contains(a.UserId));

        IEnumerable<Guid> newUserIds = allowedUserIds.Except(currentIds);
        foreach (Guid userId in newUserIds)
        {
            AllowedUsers.Add(new SectionUserAccess(
                Guid.NewGuid(),
                Id,
                userId,
                updatedBy,
                timestamp
            ));
        }

        UpdateAudit(timestamp, updatedBy);
    }
    
    public void SetDefaultPage(Guid pageId)
    {
        DefaultPageId = pageId;
    }
    
    public bool IsAccessibleTo(User user, ProjectRole? role, string? passwordHash = null)
    {
        return Visibility switch
        {
            SectionVisibility.Public => true,
            SectionVisibility.ProtectedWithPassword =>
                Password is not null && Password == passwordHash,
            SectionVisibility.ProtectedByRole =>
                role is not null && AllowedRoles?.Contains(role.Value) == true,
            SectionVisibility.ProtectedByUser =>
                AllowedUsers.Any(a => a.UserId == user.Id),
            _ => false
        };
    }
}
