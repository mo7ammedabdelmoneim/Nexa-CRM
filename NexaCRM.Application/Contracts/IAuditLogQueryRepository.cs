using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Contracts;

public interface IAuditLogQueryRepository
{
    Task<PaginatedResult<AuditLogDto>> GetPagedAsync(
        string? entityType,
        Guid? entityId,
        Guid? userId,
        string? action,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}