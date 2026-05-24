using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Deals;

namespace NexaCRM.Application.Mappings;

public static class DealMappings
{
    public static DealDto ToDto(this Deal deal) => new(
        deal.Id,
        deal.LeadId,
        deal.Title,
        deal.Stage,
        deal.Value.Amount,
        deal.Value.Currency,
        deal.CloseDate,
        deal.AssignedToUserId,
        deal.TenantId,
        deal.CreatedAt,
        deal.UpdatedAt);

    public static DealSummaryDto ToSummaryDto(this Deal deal) => new(
        deal.Id,
        deal.Title,
        deal.Stage,
        deal.Value.Amount,
        deal.Value.Currency,
        deal.CloseDate,
        deal.AssignedToUserId,
        deal.CreatedAt);
}