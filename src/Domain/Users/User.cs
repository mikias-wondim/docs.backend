using Domain.Invitations;
using Domain.ProjectMembers;
using Domain.Projects;
using SharedKernel;

namespace Domain.Users;

public sealed class User : Entity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public string? DisplayName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? Bio { get; private set; }

    public bool EmailVerified { get; private set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? LastLoginAt { get; private set; }

    // === Navigation Properties ===
    public List<Project> Projects { get; private set; } = [];
    public List<ProjectMember> ProjectMembers { get; private set; } = [];
    public List<Invitation> Invitations { get; private set; } = [];

    // Required by EF Core
    public User()
    {
    }

    public User(
        Guid id,
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        string createdBy,
        DateTime timestamp)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;

        RegisterAudit(timestamp, createdBy);
    }

    // === Domain Methods ===

    public void UpdateProfile(
        string firstName,
        string lastName,
        string? displayName,
        Uri? avatarUrl,
        string? bio,
        string updatedBy,
        DateTime timestamp)
    {
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        AvatarUrl = avatarUrl?.ToString();
        Bio = bio;

        UpdateAudit(timestamp, updatedBy);
    }

    public void ChangePassword(string newPasswordHash, string updatedBy, DateTime timestamp)
    {
        PasswordHash = newPasswordHash;
        UpdateAudit(timestamp, updatedBy);
    }

    public void VerifyEmail(DateTime timestamp)
    {
        EmailVerified = true;
        EmailVerifiedAt = timestamp;
    }

    public void RecordLogin(DateTime loginTime)
    {
        LastLoginAt = loginTime;
    }
}
