namespace NexaCRM.Domain.Events;
public sealed record LeadConvertedEvent(
    Guid LeadId,
    Guid DealId,
    string LeadName);