using NexaCRM.Domain.Common;
using NexaCRM.Domain.Exceptions;

namespace NexaCRM.Domain.Aggregates.Activities;

public sealed class Activity : BaseEntity
{
    public string Type { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }
    public Guid? LeadId { get; private set; }
    public Guid? DealId { get; private set; }
    public Guid TenantId { get; private set; }

    private Activity() { }

    public static Activity Create(
        string type,
        string description,
        DateTime occurredAt,
        Guid tenantId,
        Guid createdByUserId,
        Guid? leadId = null,
        Guid? dealId = null)
    {
        if (!ActivityType.All.Contains(type))
            throw new DomainException(
                $"Activity type must be one of: {string.Join(", ", ActivityType.All)}");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Activity description is required.");

        if (leadId is null && dealId is null)
            throw new DomainException(
                "Activity must be linked to at least a Lead or a Deal.");

        if (occurredAt > DateTime.UtcNow)
            throw new DomainException("Activity cannot be in the future.");

        return new Activity
        {
            Type = type,
            Description = description.Trim(),
            OccurredAt = occurredAt,
            TenantId = tenantId,
            LeadId = leadId,
            DealId = dealId,
            CreatedByUserId = createdByUserId,
        };
    }
}