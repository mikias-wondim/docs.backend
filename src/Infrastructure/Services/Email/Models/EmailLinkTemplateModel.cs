namespace Infrastructure.Services.Email.Models;

public class EmailLinkTemplateModel
{
    public string Link { get; init; } = null!;
    public int CurrentYear { get; init; } = DateTime.UtcNow.Year;
}
