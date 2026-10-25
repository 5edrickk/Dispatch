using Dispatch.Api.Domain;

namespace Dispatch.Api.Dtos;

public record IncidentDto(
    int Id,
    IncidentType Type,
    IncidentPriority Priority,
    IncidentStatus Status,
    string Address,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? ClosedAt
);