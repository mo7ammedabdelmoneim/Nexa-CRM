using NexaCRM.Domain.Common;

namespace NexaCRM.Domain.Events;
public sealed record LeadAssignedEvent(
    Guid LeadId,
    string LeadName,
    Guid AssignedToUserId) : IDomainEvent;