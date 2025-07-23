using Domain.Faqs;
using Domain.Feedbacks;
using Domain.Sections;
using SharedKernel;

namespace Domain.Pages;

public sealed class Page : Entity
{
    private readonly List<Page> _children = [];
    private readonly List<string> _tags = [];

    public Guid SectionId { get; private set; }
    public Guid? ParentPageId { get; private set; }
    public string Title { get; private set; }
    public string? ContentMd { get; private set; }
    public decimal Order { get; private set; }

    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
    public IReadOnlyCollection<Page> Children => _children.AsReadOnly();
    
    public Section Section { get; init; }
    public Page? ParentPage { get; init; }
    public List<Faq> Faqs { get; init; }
    public List<Feedback> Feedbacks { get; set; }
    
    public Page() { } // EF Core

    public Page(
        Guid id,
        Guid sectionId,
        string title,
        decimal order,
        string createdBy,
        DateTime createdAt,
        Guid? parentPageId = null,
        string? contentMd = null,
        IEnumerable<string>? tags = null
    )
        : base(id)
    {
        SectionId = sectionId;
        ParentPageId = parentPageId;
        Title = title;
        ContentMd = contentMd;
        Order = order;

        if (tags is not null)
        {
            _tags.AddRange(tags);
        }
        
        RegisterAudit(createdAt, createdBy);   
    }

    public void Update(
        string title,
        string? contentMd,
        IEnumerable<string>? tags,
        string updatedBy,
        DateTime timestamp
    )
    {
        Title = title;
        ContentMd = contentMd;

        _tags.Clear();
        if (tags is not null)
        {
            _tags.AddRange(tags);
        }

        UpdateAudit(timestamp, updatedBy);
    }

    public void SetOrder(decimal newOrder, string updatedBy, DateTime timestamp)
    {
        Order = newOrder;
    }

    public void SetParent(Guid? parentPageId, string updatedBy, DateTime timestamp)
    {
        ParentPageId = parentPageId;
        UpdateAudit(timestamp, updatedBy);
    }

    public void AddChild(Page childPage)
    {
        _children.Add(childPage);
    }

    public void RemoveChild(Guid childPageId)
    {
        Page? child = _children.FirstOrDefault(p => p.Id == childPageId);
        if (child is not null)
        {
            _children.Remove(child);
        }
    }
}
