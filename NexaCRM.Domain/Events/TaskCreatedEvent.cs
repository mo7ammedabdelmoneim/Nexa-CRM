using NexaCRM.Domain.Common;

namespace NexaCRM.Domain.Events;

public sealed record TaskCreatedEvent(
    Guid TaskId,
    string Title,
    Guid AssignedToUserId,
    DateTime? DueDate) : IDomainEvent;