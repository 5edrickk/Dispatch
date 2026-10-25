class Incident
{
    public int Id;
    IncidentType Type;
    public IncidentPriority Priority;
    IncidentStatus Status;
    string Description = string.Empty;
    string Address = string.Empty;
    public DateTime CreatedAt;
    public DateTime UpdatedAt;
    DateTime? ClosedAt;

    public Incident(int id, IncidentType type, IncidentPriority priority, string description, string address)
    {
        Id = id;
        Type = type;
        Priority = priority;
        Description = description;
        Address = address;
        CreatedAt = UpdatedAt = DateTime.UtcNow;
    }

    public void Transition(IncidentStatus newStatus)
    {
        if (!IncidentRules.CanTransition(Status, newStatus))
        {
            throw new InvalidStatusTransitionException(Status, newStatus);
        }
        Status = newStatus;
    }

    public void Close()
    {
        if (Status != IncidentStatus.InProgress)
        {
            throw new InvalidStatusTransitionException(Status, IncidentStatus.Closed);
        }
        Status = IncidentStatus.Closed;
        ClosedAt = DateTime.UtcNow;
    }
}