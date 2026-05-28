namespace NexaCRM.Domain.Aggregates.Audit;

public sealed class AuditLog
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public Guid UserId { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    private AuditLog() { }

    public static AuditLog Create(
        string entityType,
        Guid entityId,
        string action,
        Guid userId,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type is required.");

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action is required.");

        return new AuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            UserId = userId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
        };
    }
}