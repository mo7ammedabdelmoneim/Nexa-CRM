using Microsoft.EntityFrameworkCore;
using NexaCRM.Domain.Aggregates.Deals;
using NexaCRM.Domain.Aggregates.Leads;

namespace NexaCRM.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<LeadNote> LeadNotes => Set<LeadNote>();
    public DbSet<Deal> Deals => Set<Deal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        // Global Query Filter — Soft Delete
        modelBuilder.Entity<Lead>()
            .HasQueryFilter(l => !l.IsDeleted);
        modelBuilder.Entity<Deal>()
            .HasQueryFilter(d => !d.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }
}