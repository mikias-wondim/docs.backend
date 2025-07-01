using Domain.Users;
using SharedKernel;

namespace Domain.Sections;

public sealed class SectionUserAccess : Entity
{
    public Guid SectionId { get; private set; }
    public Guid UserId { get; private set; }

    public Section Section { get; private set; } = null!;
    public User User { get; private set; } = null!;

    // EF Core required
    public SectionUserAccess() { }

    public SectionUserAccess(Guid id, Guid sectionId, Guid userId, string createdBy, DateTime createdAt)
        : base(id)
    {
        SectionId = sectionId;
        UserId = userId;

        RegisterAudit(createdAt, createdBy);
    }

    public bool BelongsToSection(Guid sectionId) => SectionId == sectionId;

    public bool IsForUser(Guid userId) => UserId == userId;
}
