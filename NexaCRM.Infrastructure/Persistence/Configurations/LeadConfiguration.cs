using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaCRM.Domain.Aggregates.Leads;

namespace NexaCRM.Infrastructure.Persistence.Configurations;

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("Leads");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedNever();

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Source)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(l => l.TenantId)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Value Object (Owned Entity)
        builder.OwnsOne(l => l.ContactInfo, ci =>
        {
            ci.Property(c => c.Email)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(200);

            ci.Property(c => c.Phone)
                .HasColumnName("Phone")
                .HasMaxLength(20);
        });

        // Composite index for multi-tenant queries
        builder.HasIndex(l => new { l.TenantId, l.Status });
        builder.HasIndex(l => new { l.TenantId, l.AssignedToUserId });

        // Unique email per tenant
        builder.HasIndex(l => new { l.TenantId, l.IsDeleted });

        builder.HasMany(l => l.Notes)
            .WithOne()
            .HasForeignKey(n => n.LeadId)
            .OnDelete(DeleteBehavior.Cascade);

        // not persisted
        builder.Ignore(l => l.DomainEvents);
    }
}