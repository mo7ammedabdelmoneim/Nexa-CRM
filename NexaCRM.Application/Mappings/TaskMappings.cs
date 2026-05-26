using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Tasks;

namespace NexaCRM.Application.Mappings;

public static class TaskMappings
{
    public static TaskDto ToDto(this CrmTask task) => new(
        task.Id,
        task.Title,
        task.Priority,
        task.DueDate,
        task.IsCompleted,
        task.CompletedAt,
        task.AssignedToUserId,
        task.LeadId,
        task.DealId,
        task.TenantId,
        task.CreatedAt,
        task.UpdatedAt);

    public static TaskSummaryDto ToSummaryDto(this CrmTask task) => new(
        task.Id,
        task.Title,
        task.Priority,
        task.DueDate,
        task.IsCompleted,
        task.AssignedToUserId,
        task.LeadId,
        task.DealId,
        task.CreatedAt);
}