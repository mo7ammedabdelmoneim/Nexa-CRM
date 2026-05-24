namespace NexaCRM.Domain.Events;

public sealed record DealWonEvent(
    Guid DealId,
    string Title,
    decimal Amount,
    Guid? AssignedToUserId);