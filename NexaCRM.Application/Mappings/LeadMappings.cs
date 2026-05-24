using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Leads;

namespace NexaCRM.Application.Mappings;

public static class LeadMappings
{
    public static LeadDto ToDto(this Lead lead) => new(
        lead.Id,
        lead.Name,
        lead.ContactInfo.Email,
        lead.ContactInfo.Phone,
        lead.Status,
        lead.Source,
        lead.AssignedToUserId,
        lead.TenantId,
        lead.CreatedAt,
        lead.UpdatedAt);

    public static LeadSummaryDto ToSummaryDto(this Lead lead) => new(
        lead.Id,
        lead.Name,
        lead.ContactInfo.Email,
        lead.Status,
        lead.Source,
        lead.AssignedToUserId,
        lead.CreatedAt);
}