namespace NexaCRM.Application.DTOs;

public record LeadDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string Status,
    string Source,
    Guid? AssignedToUserId,
    Guid TenantId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);