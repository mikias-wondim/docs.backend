using Application.Abstractions.Services.Email;
using Domain.ProjectMembers;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Infrastructure.Services.Email.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Email;

public sealed class EmailInvitationService(
    IFluentEmail emailSender,
    IConfiguration configuration) : IEmailInvitationService
{
    public async Task SendAsync(
        string token,
        string toEmail,
        string invitedUser,
        string invitedBy,
        string projectName,
        DateTime sentAt,
        DateTime expiresAt,
        ProjectRole role,
        CancellationToken cancellationToken = default)
    {
        const string subject = "Project Invitation - ET Docs";

        string encodedToken = Uri.EscapeDataString(token);
        string invitationRespondUrl =
            $"{configuration["FrontEnd:BaseUrl"]}{configuration["FrontEnd:RespondToInvitation"]}?token={encodedToken}";

        var model = new EmailInvitationTemplateModel
        {
            Link = invitationRespondUrl,
            InvitedUser = invitedUser,
            InvitedBy = invitedBy,
            ProjectName = projectName,
            SentAt = sentAt,
            ExpiresAt = expiresAt,
            Role = role
        };

        string templatePath = Path.Combine(AppContext.BaseDirectory,
            "Services", "Email", "Templates", "InvitationEmail.cshtml");
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Email template not found at {templatePath}");
        }

        SendResponse? email = await emailSender
            .To(toEmail)
            .Subject(subject)
            .UsingTemplateFromFile(templatePath, model)
            .SendAsync(cancellationToken);

        if (!email.Successful)
        {
            throw new InvalidOperationException($"Email sending failed: {string.Join(", ", email.ErrorMessages)}");
        }
    }
}
