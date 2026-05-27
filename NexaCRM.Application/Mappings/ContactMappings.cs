using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Contacts;

namespace NexaCRM.Application.Mappings;

public static class ContactMappings
{
    public static ContactDto ToDto(this Contact contact) => new(
        contact.Id,
        contact.FirstName,
        contact.LastName,
        contact.FullName,
        contact.ContactInfo.Email,
        contact.ContactInfo.Phone,
        contact.Company,
        contact.JobTitle,
        contact.LinkedIn,
        contact.Address,
        contact.LeadId,
        contact.TenantId,
        contact.CreatedAt,
        contact.UpdatedAt);

    public static ContactSummaryDto ToSummaryDto(this Contact contact) => new(
        contact.Id,
        contact.FullName,
        contact.ContactInfo.Email,
        contact.Company,
        contact.JobTitle,
        contact.LeadId,
        contact.CreatedAt);
}