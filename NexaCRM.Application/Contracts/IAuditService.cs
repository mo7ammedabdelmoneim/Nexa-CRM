namespace NexaCRM.Application.Contracts;

public interface IAuditService
{
    Task LogAsync(
        string entityType,
        Guid entityId,
        string action,
        Guid userId,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default);
}