using Application.Features.Projects;
using Application.Features.Users;
using Domain.Invitations;
using Domain.ProjectMembers;
using SharedKernel;

namespace Application.Features.Invitations;

public sealed class InvitationResponse: EntityResponse
{
    public Guid ProjectId { get; set; }
    public Guid InvitedUserId { get; set; }
    public Guid InvitedByUserId { get; set; }

    public ProjectRole Role { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }

    public InvitationStatus Status { get; set; }

    // Navigation properties
    public ProjectSummaryResponse Project { get; set; }
    public UserSummaryResponse InvitedUser { get; set; }
    public UserSummaryResponse InvitedByUser { get; set; }
}
