using NexaCRM.Domain.Common;

namespace NexaCRM.Domain.Events;

public sealed record LeadCreatedEvent(
    Guid LeadId,
    string LeadName,
    string Email,
    Guid? AssignedToUserId) : IDomainEvent;