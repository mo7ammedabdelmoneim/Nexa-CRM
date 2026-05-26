using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaCRM.Domain.Aggregates.Tasks;

namespace NexaCRM.Infrastructure.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<CrmTask>
{
    public void Configure(EntityTypeBuilder<CrmTask> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.TenantId)
            .IsRequired();

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(t => new { t.TenantId, t.AssignedToUserId });
        builder.HasIndex(t => new { t.TenantId, t.IsCompleted });
        builder.HasIndex(t => new { t.TenantId, t.DueDate });

        builder.Ignore(t => t.DomainEvents);
    }
}