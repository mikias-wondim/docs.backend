using Application.Features.Pages;
using SharedKernel;

namespace Application.Features.Feedbacks;

public sealed class FeedbackResponse: EntityResponse
{
    public Guid PageId { get; set; }
    public string?  Comment { get; set; }
    public ushort Rating { get; set; }
    public bool IsRead { get; set; }
    
    public PageSummaryResponse Page { get; set; }
}
