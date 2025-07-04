using Domain.Pages;
using SharedKernel;

namespace Domain.Faqs;

public sealed class Faq : Entity
{
    public Guid PageId { get; private set; }
    public string Question { get; private set; }
    public string Answer { get; private set; }
    public decimal Order { get; private set; }

    public Page Page { get; set; }

    public Faq() { }

    public Faq(Guid id, Guid pageId, string question, string answer, decimal order, string createdBy, DateTime createdAt)
        : base(id)
    {
        PageId = pageId;
        Question = question;
        Answer = answer;
        Order = order;

        RegisterAudit(createdAt, createdBy);
    }

    public void Update(string question, string answer, string updatedBy, DateTime updatedAt)
    {
        Question = question;
        Answer = answer;
        UpdateAudit(updatedAt, updatedBy);
    }

    public void Reorder(decimal newOrder, string updatedBy, DateTime updatedAt)
    {
        Order = newOrder;
        UpdateAudit(updatedAt, updatedBy);
    }
}
