using Domain.Sections;
using SharedKernel;

namespace Domain.Media;

public sealed class MediaAsset : Entity
{
    public string Name { get; private set; }
    public string? AltName { get; private set; }
    public string Url { get; private set; }
    public string Type { get; private set; } 
    public Guid SectionId { get; private set; }
    public Section Section { get; set; }

    public MediaAsset() { } // EF

    public MediaAsset(Guid id, string name, string? altName, Uri url, string type, Guid sectionId, string createdBy, DateTime createdAt)
        : base(id)
    {
        Name = name;
        AltName = altName;
        Url = url.ToString();
        Type = type;
        SectionId = sectionId;

        RegisterAudit(createdAt, createdBy);
    }

    public void Update(string name, string? altName, Uri url, string type, string updatedBy, DateTime timestamp)
    {
        Name = name;
        AltName = altName;
        Url = url.ToString();
        Type = type;
        
        UpdateAudit(timestamp, updatedBy);
    }
}
