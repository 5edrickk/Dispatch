namespace Dispatch.Api.Domain;

public static class IncidentRules
{
    public static bool CanTransition(IncidentStatus from, IncidentStatus to) => from switch
    {
        IncidentStatus.New => to == IncidentStatus.Dispatched,
        IncidentStatus.Dispatched => to == IncidentStatus.InProgress,
        IncidentStatus.InProgress => to == IncidentStatus.Closed,
        _ => false
    };

    public static IncidentStatus? NextStatus(IncidentStatus current)
    {
        return current switch
        {
            IncidentStatus.New => IncidentStatus.Dispatched,
            IncidentStatus.Dispatched => IncidentStatus.InProgress,
            IncidentStatus.InProgress => IncidentStatus.Closed,
            _ => null
        };

    }

    public static IEnumerable<Incident> SortQueue(IEnumerable<Incident> incidents)
    {
        return incidents.OrderByDescending(i => i.Priority).ThenBy(i => i.CreatedAt);
    }

}