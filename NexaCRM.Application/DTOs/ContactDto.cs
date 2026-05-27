namespace NexaCRM.Application.DTOs;

public record ContactDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? Phone,
    string? Company,
    string? JobTitle,
    string? LinkedIn,
    string? Address,
    Guid? LeadId,
    Guid TenantId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
