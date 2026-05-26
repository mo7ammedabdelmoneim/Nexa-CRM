using NexaCRM.Domain.Common;
using NexaCRM.Domain.Events;
using NexaCRM.Domain.Exceptions;

namespace NexaCRM.Domain.Aggregates.Tasks;

public sealed class CrmTask : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Priority { get; private set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid AssignedToUserId { get; private set; }
    public Guid? LeadId { get; private set; }
    public Guid? DealId { get; private set; }
    public Guid TenantId { get; private set; }

    private CrmTask() { }

    public static CrmTask Create(
        string title,
        string priority,
        Guid assignedToUserId,
        Guid tenantId,
        Guid createdByUserId,
        DateTime? dueDate = null,
        Guid? leadId = null,
        Guid? dealId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title is required.");

        if (!TaskPriority.All.Contains(priority))
            throw new DomainException(
                $"Priority must be one of: {string.Join(", ", TaskPriority.All)}");

        if (leadId is null && dealId is null)
            throw new DomainException(
                "Task must be linked to at least a Lead or a Deal.");

        if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow.Date)
            throw new DomainException("Due date cannot be in the past.");

        var task = new CrmTask
        {
            Title = title.Trim(),
            Priority = priority,
            DueDate = dueDate,
            AssignedToUserId = assignedToUserId,
            TenantId = tenantId,
            LeadId = leadId,
            DealId = dealId,
            CreatedByUserId = createdByUserId,
        };

        task.RaiseDomainEvent(new TaskCreatedEvent(
            task.Id, task.Title, assignedToUserId, dueDate));

        return task;
    }

    public void Complete()
    {
        if (IsCompleted)
            throw new DomainException("Task is already completed.");

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new TaskCompletedEvent(Id, Title, AssignedToUserId));
    }

    public void UpdateDetails(string title, string priority, DateTime? dueDate)
    {
        if (IsCompleted)
            throw new DomainException("Cannot update a completed task.");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title is required.");

        if (!TaskPriority.All.Contains(priority))
            throw new DomainException(
                $"Priority must be one of: {string.Join(", ", TaskPriority.All)}");

        Title = title.Trim();
        Priority = priority;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsOverdue()
    {
        if (!IsCompleted)
            RaiseDomainEvent(new TaskOverdueEvent(Id, Title, AssignedToUserId));
    }
}