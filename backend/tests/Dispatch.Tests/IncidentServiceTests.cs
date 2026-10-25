using Dispatch.Api.Data;
using Dispatch.Api.Domain;
using Dispatch.Api.Dtos;
using Dispatch.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dispatch.Tests;

public class IncidentServiceTests
{
    // ---------- Helpers ----------

    // Fresh in-memory database for every test (unique name)
    private static DispatchDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<DispatchDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DispatchDbContext(options);
    }

    private static CreateIncidentRequest MakeRequest(
        IncidentType type = IncidentType.Fire,
        IncidentPriority priority = IncidentPriority.High) =>
        new()
        {
            Type = type,
            Priority = priority,
            Address = "12 Rue Imaginaire",
            Description = "Test incident (fictional)."
        };

    // Inserts an incident directly in the database (bypasses the service)
    private static async Task<Incident> SeedAsync(
        DispatchDbContext db,
        IncidentStatus status = IncidentStatus.New,
        IncidentPriority priority = IncidentPriority.High,
        IncidentType type = IncidentType.Fire,
        DateTime? createdAt = null)
    {
        var incident = new Incident
        {
            Type = type,
            Priority = priority,
            Status = status,
            Address = "12 Rue Imaginaire",
            Description = "Seeded incident (fictional).",
            CreatedAt = createdAt ?? DateTime.UtcNow,
            UpdatedAt = createdAt ?? DateTime.UtcNow,
            ClosedAt = status == IncidentStatus.Closed ? DateTime.UtcNow : null
        };
        db.Incidents.Add(incident);
        await db.SaveChangesAsync();
        return incident;
    }

    // ---------- CreateAsync ----------

    [Fact]
    public async Task CreateAsync_SetsNewStatusAndDates()
    {
        using var db = CreateDb();
        var service = new IncidentService(db);

        var dto = await service.CreateAsync(MakeRequest());

        Assert.True(dto.Id > 0);
        Assert.Equal(IncidentStatus.New, dto.Status);
        Assert.NotEqual(default, dto.CreatedAt);
        Assert.Equal(dto.CreatedAt, dto.UpdatedAt);
        Assert.Null(dto.ClosedAt);
    }

    // ---------- ListAsync ----------

    [Fact]
    public async Task ListAsync_NoFilter_ExcludesClosed()
    {
        using var db = CreateDb();
        await SeedAsync(db, IncidentStatus.New);
        await SeedAsync(db, IncidentStatus.Closed);
        var service = new IncidentService(db);

        var result = await service.ListAsync(new IncidentFilter());

        Assert.Single(result);
        Assert.DoesNotContain(result, i => i.Status == IncidentStatus.Closed);
    }

    [Fact]
    public async Task ListAsync_StatusClosed_IncludesClosed()
    {
        using var db = CreateDb();
        await SeedAsync(db, IncidentStatus.New);
        await SeedAsync(db, IncidentStatus.Closed);
        var service = new IncidentService(db);

        var result = await service.ListAsync(new IncidentFilter { Status = IncidentStatus.Closed });

        Assert.Single(result);
        Assert.Equal(IncidentStatus.Closed, result[0].Status);
    }

    [Fact]
    public async Task ListAsync_IncludeClosed_ReturnsEverything()
    {
        using var db = CreateDb();
        await SeedAsync(db, IncidentStatus.New);
        await SeedAsync(db, IncidentStatus.InProgress);
        await SeedAsync(db, IncidentStatus.Closed);
        var service = new IncidentService(db);

        var result = await service.ListAsync(new IncidentFilter { IncludeClosed = true });

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task ListAsync_FiltersByPriorityAndType()
    {
        using var db = CreateDb();
        await SeedAsync(db, priority: IncidentPriority.High, type: IncidentType.Fire);
        await SeedAsync(db, priority: IncidentPriority.Low, type: IncidentType.Fire);
        await SeedAsync(db, priority: IncidentPriority.High, type: IncidentType.Medical);
        var service = new IncidentService(db);

        var byPriority = await service.ListAsync(new IncidentFilter { Priority = IncidentPriority.High });
        var byType = await service.ListAsync(new IncidentFilter { Type = IncidentType.Fire });
        var both = await service.ListAsync(new IncidentFilter
        {
            Priority = IncidentPriority.High,
            Type = IncidentType.Fire
        });

        Assert.Equal(2, byPriority.Count);
        Assert.Equal(2, byType.Count);
        Assert.Single(both);
    }

    [Fact]
    public async Task ListAsync_ReturnsCorrectOrder()
    {
        using var db = CreateDb();
        var t0 = new DateTime(2026, 10, 25, 9, 0, 0, DateTimeKind.Utc);

        var lowOld = await SeedAsync(db, priority: IncidentPriority.Low, createdAt: t0);
        var highNew = await SeedAsync(db, priority: IncidentPriority.High, createdAt: t0.AddHours(2));
        var highOld = await SeedAsync(db, priority: IncidentPriority.High, createdAt: t0.AddHours(1));
        var critical = await SeedAsync(db, priority: IncidentPriority.Critical, createdAt: t0.AddHours(3));
        var service = new IncidentService(db);

        var result = await service.ListAsync(new IncidentFilter());

        Assert.Equal(
            new[] { critical.Id, highOld.Id, highNew.Id, lowOld.Id },
            result.Select(i => i.Id));
    }

    // ---------- UpdateStatusAsync ----------

    [Fact]
    public async Task UpdateStatusAsync_ValidTransition_UpdatesStatusAndUpdatedAt()
    {
        using var db = CreateDb();
        var old = DateTime.UtcNow.AddHours(-1);
        var incident = await SeedAsync(db, IncidentStatus.New, createdAt: old);
        var service = new IncidentService(db);

        var dto = await service.UpdateStatusAsync(incident.Id, IncidentStatus.Dispatched);

        Assert.NotNull(dto);
        Assert.Equal(IncidentStatus.Dispatched, dto!.Status);
        Assert.True(dto.UpdatedAt > old);
        Assert.Null(dto.ClosedAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_FullLifecycle_SetsClosedAt()
    {
        using var db = CreateDb();
        var service = new IncidentService(db);
        var created = await service.CreateAsync(MakeRequest());

        await service.UpdateStatusAsync(created.Id, IncidentStatus.Dispatched);
        await service.UpdateStatusAsync(created.Id, IncidentStatus.InProgress);
        var closed = await service.UpdateStatusAsync(created.Id, IncidentStatus.Closed);

        Assert.NotNull(closed);
        Assert.Equal(IncidentStatus.Closed, closed!.Status);
        Assert.NotNull(closed.ClosedAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_InvalidTransition_Throws()
    {
        using var db = CreateDb();
        var incident = await SeedAsync(db, IncidentStatus.New);
        var service = new IncidentService(db);

        await Assert.ThrowsAsync<InvalidStatusTransitionException>(
            () => service.UpdateStatusAsync(incident.Id, IncidentStatus.Closed));
    }

    [Fact]
    public async Task UpdateStatusAsync_UnknownId_ReturnsNull()
    {
        using var db = CreateDb();
        var service = new IncidentService(db);

        var result = await service.UpdateStatusAsync(9999, IncidentStatus.Dispatched);

        Assert.Null(result);
    }
}