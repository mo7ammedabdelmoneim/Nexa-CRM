using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Aggregates.Notifications;
using NexaCRM.Domain.Events;

namespace NexaCRM.Application.EventHandlers;

public class LeadAssignedEventHandler
    : INotificationHandler<DomainEventNotification<LeadAssignedEvent>>
{
    private readonly INotificationService _notificationService;

    public LeadAssignedEventHandler(INotificationService notificationService)
        => _notificationService = notificationService;

    public async Task Handle(
        DomainEventNotification<LeadAssignedEvent> notification,
        CancellationToken cancellationToken)
    {
        var e = notification.Event;

        await _notificationService.SendAsync(
            e.AssignedToUserId,
            $"Lead '{e.LeadName}' has been assigned to you.",
            NotificationType.LeadAssigned,
            e.LeadId,
            "Lead",
            cancellationToken);
    }
}