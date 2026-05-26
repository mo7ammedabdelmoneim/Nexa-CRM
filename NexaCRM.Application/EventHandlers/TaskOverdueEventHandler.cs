using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Aggregates.Notifications;
using NexaCRM.Domain.Events;

namespace NexaCRM.Application.EventHandlers;

public class TaskOverdueEventHandler
    : INotificationHandler<DomainEventNotification<TaskOverdueEvent>>
{
    private readonly INotificationService _notificationService;

    public TaskOverdueEventHandler(INotificationService notificationService)
        => _notificationService = notificationService;

    public async Task Handle(
        DomainEventNotification<TaskOverdueEvent> notification,
        CancellationToken cancellationToken)
    {
        var e = notification.Event;

        await _notificationService.SendAsync(
            e.AssignedToUserId,
            $"Task '{e.Title}' is overdue.",
            NotificationType.TaskOverdue,
            e.TaskId,
            "Task",
            cancellationToken);
    }
}