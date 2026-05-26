namespace NexaCRM.Application.DTOs;

public record TaskDto(
    Guid Id,
    string Title,
    string Priority,
    DateTime? DueDate,
    bool IsCompleted,
    DateTime? CompletedAt,
    Guid AssignedToUserId,
    Guid? LeadId,
    Guid? DealId,
    Guid TenantId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
