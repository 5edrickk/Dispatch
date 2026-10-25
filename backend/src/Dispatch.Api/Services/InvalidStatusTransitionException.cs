class InvalidStatusTransitionException : Exception
{
    public InvalidStatusTransitionException(IncidentStatus from, IncidentStatus to)
        : base($"Cannot transition from {from} to {to}")
    {
    }
}