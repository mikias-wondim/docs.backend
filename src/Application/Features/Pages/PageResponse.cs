using Application.Features.Sections;
using SharedKernel;

namespace Application.Features.Pages;

public sealed class PageResponse: EntityResponse
{
    public Guid SectionId { get; init; }
    public Guid? ParentPageId { get; init; }
    public string Title { get; init; } = null!;
    public string? ContentMd { get; init; }
    public decimal Order { get; init; }
    public List<string> Tags { get; init; } = [];
    public List<PageResponse> Children { get; set; } = [];
    public PageSummaryResponse? ParentPage { get; init; }
    public SectionSummerResponse Section { get; init; }
}
