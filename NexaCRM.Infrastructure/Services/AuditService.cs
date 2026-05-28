using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Aggregates.Audit;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork)
    {
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(
        string entityType,
        Guid entityId,
        string action,
        Guid userId,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = AuditLog.Create(
            entityType,
            entityId,
            action,
            userId,
            oldValues,
            newValues,
            ipAddress);

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}