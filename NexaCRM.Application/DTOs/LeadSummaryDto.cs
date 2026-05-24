namespace NexaCRM.Application.DTOs;

public record LeadSummaryDto(
    Guid Id,
    string Name,
    string Email,
    string Status,
    string Source,
    Guid? AssignedToUserId,
    DateTime CreatedAt);
