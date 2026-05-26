using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Aggregates.Notifications;

namespace NexaCRM.Infrastructure.Jobs;

public class TaskReminderJob
{
    private readonly ITaskQueryRepository _taskQueryRepository;
    private readonly INotificationService _notificationService;

    public TaskReminderJob(
        ITaskQueryRepository taskQueryRepository,
        INotificationService notificationService)
    {
        _taskQueryRepository = taskQueryRepository;
        _notificationService = notificationService;
    }

    public async Task ExecuteAsync()
    {
        var dueBefore = DateTime.UtcNow.AddHours(24);

        var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        var tasks = await _taskQueryRepository.GetPagedAsync(
            tenantId,
            assignedToUserId: null,
            isCompleted: false,
            priority: null,
            dueBefore: dueBefore,
            page: 1,
            pageSize: 100);

        foreach (var task in tasks.Items.Where(t =>
            t.DueDate.HasValue &&
            t.DueDate.Value > DateTime.UtcNow))
        {
            await _notificationService.SendAsync(
                task.AssignedToUserId,
                $"Reminder: Task '{task.Title}' is due on {task.DueDate!.Value:MMM dd, yyyy}.",
                NotificationType.TaskAssigned,
                task.Id,
                "Task");
        }
    }
}