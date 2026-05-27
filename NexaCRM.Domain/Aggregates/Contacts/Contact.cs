using NexaCRM.Domain.Common;
using NexaCRM.Domain.Events;
using NexaCRM.Domain.Exceptions;
using NexaCRM.Domain.ValueObjects;

namespace NexaCRM.Domain.Aggregates.Contacts;

public sealed class Contact : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public ContactInfo ContactInfo { get; private set; } = null!;
    public string? Company { get; private set; }
    public string? JobTitle { get; private set; }
    public string? LinkedIn { get; private set; }
    public string? Address { get; private set; }
    public Guid? LeadId { get; private set; }
    public Guid TenantId { get; private set; }

    private Contact() { }

    public static Contact Create(
        string firstName,
        string lastName,
        string email,
        Guid tenantId,
        Guid createdByUserId,
        string? phone = null,
        string? company = null,
        string? jobTitle = null,
        string? linkedIn = null,
        string? address = null,
        Guid? leadId = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        var contact = new Contact
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            ContactInfo = ContactInfo.Create(email, phone),
            TenantId = tenantId,
            Company = company?.Trim(),
            JobTitle = jobTitle?.Trim(),
            LinkedIn = linkedIn?.Trim(),
            Address = address?.Trim(),
            LeadId = leadId,
            CreatedByUserId = createdByUserId,
        };

        contact.RaiseDomainEvent(new ContactCreatedEvent(
            contact.Id, contact.FullName, email));

        return contact;
    }

    public void UpdateDetails(
        string firstName,
        string lastName,
        string email,
        string? phone = null,
        string? company = null,
        string? jobTitle = null,
        string? linkedIn = null,
        string? address = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        ContactInfo = ContactInfo.Create(email, phone);
        Company = company?.Trim();
        JobTitle = jobTitle?.Trim();
        LinkedIn = linkedIn?.Trim();
        Address = address?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToLead(Guid leadId)
    {
        LeadId = leadId;
        UpdatedAt = DateTime.UtcNow;
    }
}