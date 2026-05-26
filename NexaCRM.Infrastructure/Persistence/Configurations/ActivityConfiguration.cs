using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaCRM.Domain.Aggregates.Activities;

namespace NexaCRM.Infrastructure.Persistence.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("Activities");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.Type)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.OccurredAt)
            .IsRequired();

        builder.Property(a => a.TenantId)
            .IsRequired();

        builder.HasIndex(a => new { a.TenantId, a.LeadId });
        builder.HasIndex(a => new { a.TenantId, a.DealId });
        builder.HasIndex(a => new { a.TenantId, a.Type });

        builder.Ignore(a => a.DomainEvents);
    }
}