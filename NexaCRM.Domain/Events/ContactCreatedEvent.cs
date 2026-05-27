using NexaCRM.Domain.Common;

namespace NexaCRM.Domain.Events;

public sealed record ContactCreatedEvent(
    Guid ContactId,
    string FullName,
    string Email) : IDomainEvent;