using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Users;
using SharedKernel;

namespace Domain.Invitations;

public class Invitation : Entity
{
    public Guid ProjectId { get; private set; }
    public Guid InvitedUserId { get; private set; }
    public Guid InvitedByUserId { get; private set; }

    public ProjectRole Role { get; private set; }
    public DateTime SentAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }

    public InvitationStatus Status { get; private set; }

    // Navigation properties
    public Project Project { get; set; }
    public User InvitedUser { get; set; }
    public User InvitedByUser { get; set; }

    // Required by EF Core
    public Invitation() { }

    public Invitation(Guid id, Guid projectId, Guid invitedUserId, Guid invitedByUserId, ProjectRole role, DateTime sentAt,
        DateTime expiresAt, string createdBy)
        : base(id)
    {
        ProjectId = projectId;
        InvitedUserId = invitedUserId;
        InvitedByUserId = invitedByUserId;
        Role = role;
        SentAt = sentAt;
        ExpiresAt = expiresAt;
        Status = InvitationStatus.Pending;

        RegisterAudit(sentAt, createdBy);
    }

    public void Accept(DateTime timestamp)
    {
        AcceptedAt = timestamp;
        Status = InvitationStatus.Accepted;

        UpdateAudit(timestamp, "");
    }

    public void Reject(DateTime timestamp)
    {
        RejectedAt = timestamp;
        Status = InvitationStatus.Rejected;

        UpdateAudit(timestamp, "");
    }

    public bool IsExpired(DateTime now) => Status == InvitationStatus.Pending && now > ExpiresAt;

    public bool IsPending() => Status == InvitationStatus.Pending;
}
