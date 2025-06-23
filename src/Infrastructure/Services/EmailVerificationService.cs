using Application.Abstractions.Services;

namespace Infrastructure.Services;

public class EmailVerificationService(
    IEmailService emailService): IEmailVerificationService
{
    public async Task SendVerificationEmailAsync(string token, string email, CancellationToken cancellationToken = default)
    {
        string verificationUrl = $"http://localhost:5001/auth/verify-email?token={token}";
        string body = $"Please verify your email by clicking <a href='{verificationUrl}'>here</a>.";
        const string subject = "Email Verification - ET Docs";

        await emailService.SendAsync(email, subject, body, true, cancellationToken);
    }
}
