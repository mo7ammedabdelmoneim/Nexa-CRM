using NexaCRM.Domain.Common;

namespace NexaCRM.Domain.Events;

public sealed record TaskOverdueEvent(
    Guid TaskId,
    string Title,
    Guid AssignedToUserId) : IDomainEvent;