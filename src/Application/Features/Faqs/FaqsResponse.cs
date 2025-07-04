using Application.Features.Pages;
using SharedKernel;

namespace Application.Features.Faqs;

public sealed class FaqResponse: EntityResponse
{
    public Guid PageId { get; set; }
    public string Question { get; init; }
    public string Answer { get; init; }
    public decimal Order { get; init; }
    
    public PageSummaryResponse Page { get; init; } = null!;
}
