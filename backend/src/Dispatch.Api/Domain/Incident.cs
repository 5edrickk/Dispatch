namespace Dispatch.Api.Domain;

public class Incident
{
    public int Id { get; set; }
    public IncidentType Type { get; set; }
    public IncidentPriority Priority { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.New;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}