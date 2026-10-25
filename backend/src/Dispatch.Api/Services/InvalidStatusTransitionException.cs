using Dispatch.Api.Domain;

namespace Dispatch.Api.Services;

public class InvalidStatusTransitionException : Exception
{
    public InvalidStatusTransitionException(IncidentStatus from, IncidentStatus to)
        : base($"Cannot change status from {from} to {to}.")
    {
    }
}