using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Aggregates.Notifications;
using NexaCRM.Domain.Events;

namespace NexaCRM.Application.EventHandlers;

public class DealStageChangedEventHandler
    : INotificationHandler<DomainEventNotification<DealStageChangedEvent>>
{
    private readonly INotificationService _notificationService;

    public DealStageChangedEventHandler(INotificationService notificationService)
        => _notificationService = notificationService;

    public async Task Handle(
        DomainEventNotification<DealStageChangedEvent> notification,
        CancellationToken cancellationToken)
    {
        var e = notification.Event;

        if (e.AssignedToUserId is null) return;

        await _notificationService.SendAsync(
            e.AssignedToUserId.Value,
            $"Deal '{e.Title}' moved from {e.OldStage} to {e.NewStage}.",
            NotificationType.DealStageChanged,
            e.DealId,
            "Deal",
            cancellationToken);
    }
}