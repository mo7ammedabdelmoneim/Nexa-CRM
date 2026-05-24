namespace NexaCRM.Application.DTOs;

public record DealSummaryDto(
    Guid Id,
    string Title,
    string Stage,
    decimal Amount,
    string Currency,
    DateTime? CloseDate,
    Guid? AssignedToUserId,
    DateTime CreatedAt);
