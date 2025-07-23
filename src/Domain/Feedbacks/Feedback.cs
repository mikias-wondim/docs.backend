using Domain.Pages;
using SharedKernel;

namespace Domain.Feedbacks;

public sealed class Feedback: Entity
{
    public Guid PageId { get; private set; }
    public string?  Comment { get; private set; }
    public ushort Rating { get; private set; }
    public bool IsRead { get; private set; }
    
    public Page Page { get; set; }
    
    public Feedback(Guid id, Guid pageId, string? comment, ushort rating, DateTime createdAt)
        : base(id)
    {
        PageId = pageId;
        Comment = comment;
        Rating = rating;
        IsRead = false;
        
        RegisterAudit(createdAt, "");
    }
    
    public void MarkAsRead(string updatedBy, DateTime timestamp)
    {
        IsRead = true;
        
        UpdateAudit(timestamp, updatedBy);
    }
}
