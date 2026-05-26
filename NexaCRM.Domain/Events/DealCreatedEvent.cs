using NexaCRM.Domain.Common;

namespace NexaCRM.Domain.Events;

public sealed record DealCreatedEvent(
    Guid DealId,
    Guid LeadId,
    string Title,
    decimal Amount,
    Guid? AssignedToUserId) : IDomainEvent;
