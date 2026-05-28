using Microsoft.EntityFrameworkCore;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class AuditLogQueryRepository : IAuditLogQueryRepository
{
    private readonly AppDbContext _context;

    public AuditLogQueryRepository(AppDbContext context)
        => _context = context;

    public async Task<PaginatedResult<AuditLogDto>> GetPagedAsync(
        string? entityType,
        Guid? entityId,
        Guid? userId,
        string? action,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(a => a.EntityType == entityType);

        if (entityId.HasValue)
            query = query.Where(a => a.EntityId == entityId);

        if (userId.HasValue)
            query = query.Where(a => a.UserId == userId);

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action == action);

        if (from.HasValue)
            query = query.Where(a => a.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(a => a.Timestamp <= to.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditLogDto(
                a.Id,
                a.EntityType,
                a.EntityId,
                a.Action,
                a.OldValues,
                a.NewValues,
                a.UserId,
                a.IpAddress,
                a.Timestamp))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<AuditLogDto>(items, totalCount, page, pageSize);
    }
}