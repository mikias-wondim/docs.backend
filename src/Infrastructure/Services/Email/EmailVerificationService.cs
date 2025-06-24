using Application.Abstractions.Services.Email;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Infrastructure.Services.Email;

public class EmailVerificationService(
    IFluentEmail emailSender,
    IEmailLinkGenerator linkGenerator) : IEmailVerificationService
{
    public async Task SendVerificationEmailAsync(string token, string toEmail,
        CancellationToken cancellationToken = default)
    {
        const string subject = "Email Verification - ET Docs";
        string verificationUrl = linkGenerator.GenerateVerificationLink(token);

        var model = new EmailVerificationTemplateModel { VerificationLink = verificationUrl };

        string templatePath = Path.Combine(AppContext.BaseDirectory,
            "Services",
            "Email",
            "Templates",
            "VerifyEmail.cshtml");
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
