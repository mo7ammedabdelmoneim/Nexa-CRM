using NexaCRM.Domain.Common;

namespace NexaCRM.Domain.Aggregates.Notifications;

public sealed class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public Guid? EntityId { get; private set; }
    public string? EntityType { get; private set; }

    private Notification() { }

    public static Notification Create(
        Guid userId,
        string message,
        string type,
        Guid? entityId = null,
        string? entityType = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Notification message is required.");

        return new Notification
        {
            UserId = userId,
            Message = message.Trim(),
            Type = type,
            IsRead = false,
            EntityId = entityId,
            EntityType = entityType,
        };
    }

    public void MarkAsRead()
    {
        IsRead = true;
        UpdatedAt = DateTime.UtcNow;
    }
}