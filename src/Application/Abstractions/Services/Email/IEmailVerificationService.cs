namespace Application.Abstractions.Services.Email;

public interface IEmailVerificationService
{
    Task SendVerificationEmailAsync(string token, string toEmail,
        CancellationToken cancellationToken = default);
}
