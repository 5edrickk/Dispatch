using Dispatch.Api.Domain;

namespace Dispatch.Api.Data;

public static class SeedData
{
    public static void Initialize(DispatchDbContext db)
    {
        if (db.Incidents.Any())
        {
            return; // data already exists, do not duplicate
        }

        var now = DateTime.UtcNow;

        db.Incidents.AddRange(
            new Incident
            {
                Type = IncidentType.Fire,
                Priority = IncidentPriority.Critical,
                Status = IncidentStatus.New,
                Address = "12 Rue Imaginaire",
                Description = "Smoke coming from a warehouse (fictional).",
                CreatedAt = now.AddMinutes(-50),
                UpdatedAt = now.AddMinutes(-50)
            },
            new Incident
            {
                Type = IncidentType.Medical,
                Priority = IncidentPriority.High,
                Status = IncidentStatus.Dispatched,
                Address = "300 Avenue de Test",
                Description = "Person feeling dizzy at a bus stop (fictional).",
                CreatedAt = now.AddMinutes(-40),
                UpdatedAt = now.AddMinutes(-35)
            },
            new Incident
            {
                Type = IncidentType.Police,
                Priority = IncidentPriority.Medium,
                Status = IncidentStatus.InProgress,
                Address = "45 Boulevard Fictif",
                Description = "Noise complaint at a closed shop (fictional).",
                CreatedAt = now.AddMinutes(-30),
                UpdatedAt = now.AddMinutes(-20)
            },
            new Incident
            {
                Type = IncidentType.Municipal,
                Priority = IncidentPriority.Low,
                Status = IncidentStatus.New,
                Address = "8 Chemin Exemple",
                Description = "Streetlight not working (fictional).",
                CreatedAt = now.AddMinutes(-25),
                UpdatedAt = now.AddMinutes(-25)
            },
            new Incident
            {
                Type = IncidentType.Fire,
                Priority = IncidentPriority.High,
                Status = IncidentStatus.New,
                Address = "77 Rue du Faux Nom",
                Description = "Burning smell reported in an apartment hall (fictional).",
                CreatedAt = now.AddMinutes(-15),
                UpdatedAt = now.AddMinutes(-15)
            },
            new Incident
            {
                Type = IncidentType.Municipal,
                Priority = IncidentPriority.Medium,
                Status = IncidentStatus.Closed,
                Address = "21 Place Inventée",
                Description = "Fallen tree blocking a lane, already cleared (fictional).",
                CreatedAt = now.AddMinutes(-120),
                UpdatedAt = now.AddMinutes(-60),
                ClosedAt = now.AddMinutes(-60)
            }
        );

        db.SaveChanges();
    }
}