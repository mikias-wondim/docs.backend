using Application.Abstractions.Services.Email;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Infrastructure.Services.Email.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Email;

public sealed class EmailVerificationService(
    IFluentEmail emailSender,
    IConfiguration configuration) : IEmailVerificationService
{
    public async Task SendAsync(string token, string toEmail,
        CancellationToken cancellationToken = default)
    {
        const string subject = "Email Verification - ET Docs";
        string encodedToken = Uri.EscapeDataString(token);
        string verificationUrl =
            $"{configuration["FrontEnd:BaseUrl"]}{configuration["FrontEnd:VerifyEmail"]}?token={encodedToken}";


        var model = new EmailLinkTemplateModel { Link = verificationUrl };

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
