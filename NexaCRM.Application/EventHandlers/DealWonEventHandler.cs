using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Aggregates.Notifications;
using NexaCRM.Domain.Events;

namespace NexaCRM.Application.EventHandlers;

public class DealWonEventHandler
    : INotificationHandler<DomainEventNotification<DealWonEvent>>
{
    private readonly INotificationService _notificationService;

    public DealWonEventHandler(INotificationService notificationService)
        => _notificationService = notificationService;

    public async Task Handle(
        DomainEventNotification<DealWonEvent> notification,
        CancellationToken cancellationToken)
    {
        var e = notification.Event;

        if (e.AssignedToUserId is null) return;

        await _notificationService.SendAsync(
            e.AssignedToUserId.Value,
            $"Deal '{e.Title}' worth {e.Amount:C} has been WON!",
            NotificationType.DealWon,
            e.DealId,
            "Deal",
            cancellationToken);
    }
}