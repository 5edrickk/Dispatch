using Dispatch.Api.Domain;

namespace Dispatch.Api.Dtos;

public static class IncidentMappings
{
    public static IncidentDto ToDto(this Incident incident) =>
        new IncidentDto(
            incident.Id,
            incident.Type,
            incident.Priority,
            incident.Status,
            incident.Address,
            incident.Description,
            incident.CreatedAt,
            incident.UpdatedAt,
            incident.ClosedAt
        );
}