namespace Infrastructure.Services.Email;

public class EmailVerificationTemplateModel
{
    public string VerificationLink { get; init; } = null!;
    public int CurrentYear { get; init; } = DateTime.UtcNow.Year;
}
