namespace Application.Abstractions.Services.Email;

public interface IEmailVerificationService
{
    Task SendAsync(string token, string toEmail,
        CancellationToken cancellationToken = default);
}
