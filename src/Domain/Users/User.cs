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
    public DateTime? LastLoginAt { get; private set; }

    // === Navigation Properties ===
    public List<Project> Projects { get; private set; } = [];

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

        Raise(new UserRegisteredDomainEvent(Id));
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

    public void VerifyEmail(DateTime timestamp, string updatedBy)
    {
        EmailVerified = true;
        UpdateAudit(timestamp, updatedBy);
    }

    public void RecordLogin(DateTime loginTime)
    {
        LastLoginAt = loginTime;
    }
}
