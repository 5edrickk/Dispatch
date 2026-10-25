using Xunit;
using Dispatch.Api.Domain;

namespace Dispatch.Tests;
using Dispatch.Api.Services;

public class IncidentRulesTests
{
    // ---------- CanTransition: 16 combinations (3 true, 13 false) ----------

    [Theory]
    [InlineData(IncidentStatus.New, IncidentStatus.Dispatched, true)]
    [InlineData(IncidentStatus.New, IncidentStatus.New, false)]
    [InlineData(IncidentStatus.New, IncidentStatus.InProgress, false)]
    [InlineData(IncidentStatus.New, IncidentStatus.Closed, false)]
    [InlineData(IncidentStatus.Dispatched, IncidentStatus.New, false)]
    [InlineData(IncidentStatus.Dispatched, IncidentStatus.Dispatched, false)]
    [InlineData(IncidentStatus.Dispatched, IncidentStatus.InProgress, true)]
    [InlineData(IncidentStatus.Dispatched, IncidentStatus.Closed, false)]
    [InlineData(IncidentStatus.InProgress, IncidentStatus.New, false)]
    [InlineData(IncidentStatus.InProgress, IncidentStatus.Dispatched, false)]
    [InlineData(IncidentStatus.InProgress, IncidentStatus.InProgress, false)]
    [InlineData(IncidentStatus.InProgress, IncidentStatus.Closed, true)]
    [InlineData(IncidentStatus.Closed, IncidentStatus.New, false)]
    [InlineData(IncidentStatus.Closed, IncidentStatus.Dispatched, false)]
    [InlineData(IncidentStatus.Closed, IncidentStatus.InProgress, false)]
    [InlineData(IncidentStatus.Closed, IncidentStatus.Closed, false)]
    public void CanTransition_ReturnsExpected(IncidentStatus from, IncidentStatus to, bool expected)
    {
        bool result = IncidentRules.CanTransition(from, to);
        Assert.Equal(expected, result);
    }

    // ---------- NextStatus ----------

    [Theory]
    [InlineData(IncidentStatus.New, IncidentStatus.Dispatched)]
    [InlineData(IncidentStatus.Dispatched, IncidentStatus.InProgress)]
    [InlineData(IncidentStatus.InProgress, IncidentStatus.Closed)]
    [InlineData(IncidentStatus.Closed, null)]
    public void NextStatus_ReturnsExpected(IncidentStatus current, IncidentStatus? expected)
    {
        IncidentStatus? result = IncidentRules.NextStatus(current);

        Assert.Equal(expected, result);
    }

    // ---------- SortQueue ----------

    [Fact]
    public void SortQueue_PutsHighestPriorityFirst()
    {
        var low      = new Incident { Id = 1, Priority = IncidentPriority.Low };
        var critical = new Incident { Id = 2, Priority = IncidentPriority.Critical };
        var medium   = new Incident { Id = 3, Priority = IncidentPriority.Medium };

        var result = IncidentRules.SortQueue(new[] { low, critical, medium }).ToList();

        Assert.Equal(new[] { 2, 3, 1 }, result.Select(i => i.Id));
    }

    [Fact]
    public void SortQueue_SamePriority_OldestFirst()
    {
        var newer = new Incident
        {
            Id = 1,
            Priority = IncidentPriority.High,
            CreatedAt = new DateTime(2026, 10, 25, 10, 0, 0, DateTimeKind.Utc)
        };
        var older = new Incident
        {
            Id = 2,
            Priority = IncidentPriority.High,
            CreatedAt = new DateTime(2026, 10, 25, 9, 0, 0, DateTimeKind.Utc)
        };

        var result = IncidentRules.SortQueue(new[] { newer, older }).ToList();

        Assert.Equal(new[] { 2, 1 }, result.Select(i => i.Id));
    }

    [Fact]
    public void SortQueue_EmptyList_ReturnsEmpty()
    {
        var result = IncidentRules.SortQueue(new List<Incident>());

        Assert.Empty(result);
    }

    // ---------- InvalidStatusTransitionException ----------

    [Fact]
    public void InvalidStatusTransitionException_MessageContainsBothStatuses()
    {
        var ex = new InvalidStatusTransitionException(IncidentStatus.New, IncidentStatus.Closed);

        Assert.Contains("New", ex.Message);
        Assert.Contains("Closed", ex.Message);
    }
}