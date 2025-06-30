using Domain.ProjectMembers;

namespace Infrastructure.Services.Email.Models;

public sealed class EmailInvitationTemplateModel: EmailLinkTemplateModel
{
    public string InvitedUser { get; set; }
    public string InvitedBy { get; init; } 
    public string ProjectName { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public ProjectRole Role { get; set; }
}
