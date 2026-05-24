using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaCRM.Domain.Aggregates.Deals;

namespace NexaCRM.Infrastructure.Persistence.Configurations;

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("Deals");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedNever();

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Stage)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.TenantId)
            .IsRequired();

        builder.Property(d => d.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.OwnsOne(d => d.Value, v =>
        {
            v.Property(m => m.Amount)
                .HasColumnName("Amount")
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            v.Property(m => m.Currency)
                .HasColumnName("Currency")
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.HasIndex(d => new { d.TenantId, d.Stage });
        builder.HasIndex(d => new { d.TenantId, d.AssignedToUserId });

        builder.Ignore(d => d.DomainEvents);
    }
}