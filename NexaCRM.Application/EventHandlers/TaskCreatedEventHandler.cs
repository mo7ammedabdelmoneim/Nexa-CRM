using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Aggregates.Notifications;
using NexaCRM.Domain.Events;

namespace NexaCRM.Application.EventHandlers;

public class TaskCreatedEventHandler
    : INotificationHandler<DomainEventNotification<TaskCreatedEvent>>
{
    private readonly INotificationService _notificationService;

    public TaskCreatedEventHandler(INotificationService notificationService)
        => _notificationService = notificationService;

    public async Task Handle(
        DomainEventNotification<TaskCreatedEvent> notification,
        CancellationToken cancellationToken)
    {
        var e = notification.Event;

        await _notificationService.SendAsync(
            e.AssignedToUserId,
            $"New task assigned to you: '{e.Title}'." +
            (e.DueDate.HasValue
                ? $" Due: {e.DueDate.Value:MMM dd, yyyy}."
                : string.Empty),
            NotificationType.TaskAssigned,
            e.TaskId,
            "Task",
            cancellationToken);
    }
}