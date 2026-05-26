namespace NexaCRM.Application.DTOs;

public record NotificationDto(
    Guid Id,
    string Message,
    string Type,
    bool IsRead,
    Guid? EntityId,
    string? EntityType,
    DateTime CreatedAt);