namespace NexaCRM.Domain.Aggregates.Audit;

public static class AuditAction
{
    public const string Created = "Created";
    public const string Updated = "Updated";
    public const string Deleted = "Deleted";
    public const string StatusChanged = "StatusChanged";
    public const string Assigned = "Assigned";
    public const string Converted = "Converted";
}