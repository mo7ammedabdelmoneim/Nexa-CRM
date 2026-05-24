namespace NexaCRM.Application.DTOs;

public record DealDto(
    Guid Id,
    Guid LeadId,
    string Title,
    string Stage,
    decimal Amount,
    string Currency,
    DateTime? CloseDate,
    Guid? AssignedToUserId,
    Guid TenantId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
