using Dispatch.Api.Data;
using Dispatch.Api.Domain;
using Dispatch.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Dispatch.Api.Services;

public class IncidentService : IIncidentService
{
    private readonly DispatchDbContext _db;

    public IncidentService(DispatchDbContext db)
    {
        _db = db;
    }

    public async Task<IncidentDto> CreateAsync(CreateIncidentRequest request)
    {
        var now = DateTime.UtcNow;

        var incident = new Incident
        {
            Type = request.Type,
            Priority = request.Priority,
            Status = IncidentStatus.New,
            Address = request.Address,
            Description = request.Description,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Incidents.Add(incident);
        await _db.SaveChangesAsync();

        return incident.ToDto();
    }

    public async Task<IReadOnlyList<IncidentDto>> ListAsync(IncidentFilter filter)
    {
        IQueryable<Incident> query = _db.Incidents;

        if (filter.Status.HasValue)
        {
            // Explicit status: includeClosed is ignored (rule 5.5)
            query = query.Where(i => i.Status == filter.Status.Value);
        }
        else if (!filter.IncludeClosed)
        {
            query = query.Where(i => i.Status != IncidentStatus.Closed);
        }

        if (filter.Priority.HasValue)
        {
            query = query.Where(i => i.Priority == filter.Priority.Value);
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(i => i.Type == filter.Type.Value);
        }

        var incidents = await query.ToListAsync();

        return IncidentRules.SortQueue(incidents)
            .Select(i => i.ToDto())
            .ToList();
    }

    public async Task<IncidentDto?> GetAsync(int id)
    {
        var incident = await _db.Incidents.FindAsync(id);
        return incident?.ToDto();
    }

    public async Task<IncidentDto?> UpdateStatusAsync(int id, IncidentStatus newStatus)
    {
        var incident = await _db.Incidents.FindAsync(id);
        if (incident is null)
        {
            return null;
        }

        if (!IncidentRules.CanTransition(incident.Status, newStatus))
        {
            throw new InvalidStatusTransitionException(incident.Status, newStatus);
        }

        var now = DateTime.UtcNow;
        incident.Status = newStatus;
        incident.UpdatedAt = now;

        if (newStatus == IncidentStatus.Closed)
        {
            incident.ClosedAt = now;
        }

        await _db.SaveChangesAsync();

        return incident.ToDto();
    }
}