namespace SharedKernel;

public class EntityResponse
{
    public Guid Id { get; init; }
    
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }

    public RecordStatus RecordStatus { get; set; }
    
    // Intentionally left out delete audit,
    // Deleted records will not be reflected since they are filtered-out in EntityConfiguration query filter
}
