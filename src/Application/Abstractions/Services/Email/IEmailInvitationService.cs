using Domain.ProjectMembers;

namespace Application.Abstractions.Services.Email;

public interface IEmailInvitationService
{
    Task SendAsync(
        string token,
        string toEmail,
        string invitedUser,
        string invitedBy,
        string projectName,
        DateTime sentAt,
        DateTime expiresAt,
        ProjectRole role,
        CancellationToken cancellationToken = default);
}
