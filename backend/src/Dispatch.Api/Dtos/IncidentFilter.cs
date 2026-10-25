using Dispatch.Api.Domain;

namespace Dispatch.Api.Dtos;

public class IncidentFilter
{
    public IncidentStatus? Status { get; set; }
    public IncidentPriority? Priority { get; set; }
    public IncidentType? Type { get; set; }
    public bool IncludeClosed { get; set; } = false;
}