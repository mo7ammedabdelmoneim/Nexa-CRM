using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaCRM.Domain.Aggregates.Leads;

namespace NexaCRM.Infrastructure.Persistence.Configurations;

public class LeadNoteConfiguration : IEntityTypeConfiguration<LeadNote>
{
    public void Configure(EntityTypeBuilder<LeadNote> builder)
    {
        builder.ToTable("LeadNotes");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .ValueGeneratedNever();

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.CreatedByUserId)
            .IsRequired();
    }
}