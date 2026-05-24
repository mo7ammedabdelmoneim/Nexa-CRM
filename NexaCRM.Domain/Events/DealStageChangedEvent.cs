namespace NexaCRM.Domain.Events;

public sealed record DealStageChangedEvent(
    Guid DealId,
    string Title,
    string OldStage,
    string NewStage,
    Guid? AssignedToUserId);