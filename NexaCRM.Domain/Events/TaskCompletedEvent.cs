using NexaCRM.Domain.Common;

namespace NexaCRM.Domain.Events;

public sealed record TaskCompletedEvent(
    Guid TaskId,
    string Title,
    Guid AssignedToUserId):IDomainEvent;