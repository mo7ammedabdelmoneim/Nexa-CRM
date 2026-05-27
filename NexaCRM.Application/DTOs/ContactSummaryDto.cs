namespace NexaCRM.Application.DTOs;

public record ContactSummaryDto(
    Guid Id,
    string FullName,
    string Email,
    string? Company,
    string? JobTitle,
    Guid? LeadId,
    DateTime CreatedAt);