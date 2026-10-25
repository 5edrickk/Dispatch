using Dispatch.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dispatch.Api.Data;

public class DispatchDbContext : DbContext
{
    public DispatchDbContext(DbContextOptions<DispatchDbContext> options) : base(options)
    {
    }

    public DbSet<Incident> Incidents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var incident = modelBuilder.Entity<Incident>();

        incident.HasKey(i => i.Id);
        incident.Property(i => i.Id).ValueGeneratedOnAdd();
        incident.Property(i => i.Address).IsRequired().HasMaxLength(200);
        incident.Property(i => i.Description).IsRequired().HasMaxLength(1000);
    }
}