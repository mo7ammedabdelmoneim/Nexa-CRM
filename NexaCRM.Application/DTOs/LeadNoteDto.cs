namespace NexaCRM.Application.DTOs;

public record LeadNoteDto(
    Guid Id,
    string Content,
    Guid CreatedByUserId,
    DateTime CreatedAt);