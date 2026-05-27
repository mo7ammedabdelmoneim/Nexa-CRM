using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaCRM.Domain.Aggregates.Contacts;

namespace NexaCRM.Infrastructure.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Company)
            .HasMaxLength(100);

        builder.Property(c => c.JobTitle)
            .HasMaxLength(100);

        builder.Property(c => c.LinkedIn)
            .HasMaxLength(200);

        builder.Property(c => c.Address)
            .HasMaxLength(300);

        builder.Property(c => c.TenantId)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.OwnsOne(c => c.ContactInfo, ci =>
        {
            ci.Property(x => x.Email)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(200);

            ci.Property(x => x.Phone)
                .HasColumnName("Phone")
                .HasMaxLength(20);
        });

        builder.HasIndex(c => new { c.TenantId, c.IsDeleted });
        builder.HasIndex(c => new { c.TenantId, c.LeadId });

        builder.Ignore(c => c.DomainEvents);
        builder.Ignore(c => c.FullName);
    }
}