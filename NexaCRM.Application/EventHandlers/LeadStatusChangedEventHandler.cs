using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Aggregates.Notifications;
using NexaCRM.Domain.Events;

namespace NexaCRM.Application.EventHandlers;

public class LeadStatusChangedEventHandler
    : INotificationHandler<DomainEventNotification<LeadStatusChangedEvent>>
{
    private readonly INotificationService _notificationService;

    public LeadStatusChangedEventHandler(INotificationService notificationService)
        => _notificationService = notificationService;

    public async Task Handle(
        DomainEventNotification<LeadStatusChangedEvent> notification,
        CancellationToken cancellationToken)
    {
        var e = notification.Event;

        if (e.AssignedToUserId is null) return;

        await _notificationService.SendAsync(
            e.AssignedToUserId.Value,
            $"Lead '{e.LeadName}' moved from {e.OldStatus} to {e.NewStatus}.",
            NotificationType.LeadStatusChanged,
            e.LeadId,
            "Lead",
            cancellationToken);
    }
}