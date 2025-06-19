namespace SharedKernel;

public abstract class Entity(Guid id)
{
    // Constructor Overload
    protected Entity() : this(Guid.NewGuid()) { }

    public Guid Id { get; init; } = id == Guid.Empty 
        ? throw new ArgumentException("Id cannot be empty", nameof(id)) : id;

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; } = DateTime.MaxValue;

    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;

    public DateTime UpdatedAt { get; private set; }
    public string UpdatedBy { get; private set; } = string.Empty;
    
    public DateTime DeletedAt { get; private set; }
    public string DeletedBy { get; private set; } = string.Empty;

    public RecordStatus RecordStatus { get; private set; } = RecordStatus.Active;

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    public void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    protected void RegisterAudit(DateTime timestamp, string createdBy)
    {
        StartDate = timestamp;
        CreatedAt = timestamp;
        CreatedBy = createdBy;

        UpdatedAt = timestamp;
        UpdatedBy = createdBy;
    }

    protected void UpdateAudit(DateTime timestamp, string updatedBy)
    {
        UpdatedAt = timestamp;
        UpdatedBy = updatedBy;
    }

    public void UpdateRecordStatus(RecordStatus status, DateTime timestamp, string updatedBy)
    {
        RecordStatus = status;
        UpdatedAt = timestamp;
        UpdatedBy = updatedBy;
    }

    public void Delete(DateTime timestamp, string deletedBy)
    {
        EndDate = timestamp;
        RecordStatus = RecordStatus.Deleted;
        DeletedAt = timestamp;
        DeletedBy = deletedBy;
    }
}
