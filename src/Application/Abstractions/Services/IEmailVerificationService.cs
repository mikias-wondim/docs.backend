namespace Application.Abstractions.Services;

public interface IEmailVerificationService
{
    Task SendVerificationEmailAsync(string token, string email,
        CancellationToken cancellationToken = default);
}
