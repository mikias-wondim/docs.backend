namespace Application.Features.Pages;

public sealed class PageSummaryResponse
{
    public Guid SectionId { get; init; }
    public Guid? ParentPageId { get; init; }
    public string Title { get; init; } = null!;
    public string? ContentMd { get; init; }
    public decimal Order { get; init; }
    public List<string> Tags { get; init; } = [];
}
