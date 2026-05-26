namespace NexaCRM.Application.DTOs;

public record ActivityDto(
    Guid Id,
    string Type,
    string Description,
    DateTime OccurredAt,
    Guid? LeadId,
    Guid? DealId,
    Guid CreatedByUserId,
    DateTime CreatedAt);
