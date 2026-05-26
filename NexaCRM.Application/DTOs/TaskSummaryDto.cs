namespace NexaCRM.Application.DTOs;

public record TaskSummaryDto(
    Guid Id,
    string Title,
    string Priority,
    DateTime? DueDate,
    bool IsCompleted,
    Guid AssignedToUserId,
    Guid? LeadId,
    Guid? DealId,
    DateTime CreatedAt);