using NexaCRM.Domain.Aggregates.Audit;

namespace NexaCRM.Domain.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
}