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



public record AuditLogDto(
    Guid Id,
    string EntityType,
    Guid EntityId,
    string Action,
    string? OldValues,
    string? NewValues,
    Guid UserId,
    string? IpAddress,
    DateTime Timestamp);