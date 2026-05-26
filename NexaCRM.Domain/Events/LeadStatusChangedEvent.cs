using NexaCRM.Domain.Common;

namespace NexaCRM.Domain.Events;
public sealed record LeadStatusChangedEvent(
    Guid LeadId,
    string LeadName,
    string OldStatus,
    string NewStatus,
    Guid? AssignedToUserId) : IDomainEvent;