namespace NexaCRM.Application.Contracts;

public interface INotificationService
{
    Task SendAsync(
        Guid userId,
        string message,
        string type,
        Guid? entityId = null,
        string? entityType = null,
        CancellationToken cancellationToken = default);
}