namespace Application.Abstractions.Services.Email;

public interface IEmailLinkGenerator
{
    string GenerateVerificationLink(string token);
}
