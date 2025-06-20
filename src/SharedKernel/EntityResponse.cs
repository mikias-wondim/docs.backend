namespace SharedKernel;

public class EntityResponse
{
    public Guid Id { get; init; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    
    public DateTime DeletedAt { get; set; }
    public string DeletedBy { get; set; }

    public RecordStatus RecordStatus { get; set; }
}
