using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Activities;

namespace NexaCRM.Application.Mappings;

public static class ActivityMappings
{
    public static ActivityDto ToDto(this Activity activity) => new(
        activity.Id,
        activity.Type,
        activity.Description,
        activity.OccurredAt,
        activity.LeadId,
        activity.DealId,
        activity.CreatedByUserId!.Value,
        activity.CreatedAt);
}