using NexaCRM.Domain.Aggregates.Audit;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
        => _context = context;

    public async Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default)
        => await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
}