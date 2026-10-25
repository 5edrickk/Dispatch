using Dispatch.Api.Domain;
using Dispatch.Api.Dtos;

namespace Dispatch.Api.Services;

public interface IIncidentService
{
    Task<IncidentDto> CreateAsync(CreateIncidentRequest request);
    Task<IReadOnlyList<IncidentDto>> ListAsync(IncidentFilter filter);
    Task<IncidentDto?> GetAsync(int id);
    Task<IncidentDto?> UpdateStatusAsync(int id, IncidentStatus newStatus);
}