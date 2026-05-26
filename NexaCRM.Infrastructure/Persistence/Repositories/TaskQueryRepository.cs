using Microsoft.EntityFrameworkCore;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class TaskQueryRepository : ITaskQueryRepository
{
    private readonly AppDbContext _context;

    public TaskQueryRepository(AppDbContext context)
        => _context = context;

    public async Task<PaginatedResult<TaskSummaryDto>> GetPagedAsync(
        Guid tenantId,
        Guid? assignedToUserId,
        bool? isCompleted,
        string? priority,
        DateTime? dueBefore,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => t.TenantId == tenantId);

        if (assignedToUserId.HasValue)
            query = query.Where(t => t.AssignedToUserId == assignedToUserId);

        if (isCompleted.HasValue)
            query = query.Where(t => t.IsCompleted == isCompleted.Value);

        if (!string.IsNullOrWhiteSpace(priority))
            query = query.Where(t => t.Priority == priority);

        if (dueBefore.HasValue)
            query = query.Where(t => t.DueDate <= dueBefore.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(t => t.DueDate)
            .ThenByDescending(t => t.Priority)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskSummaryDto(
                t.Id,
                t.Title,
                t.Priority,
                t.DueDate,
                t.IsCompleted,
                t.AssignedToUserId,
                t.LeadId,
                t.DealId,
                t.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<TaskSummaryDto>(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<TaskSummaryDto>> GetOverdueAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _context.Tasks
            .AsNoTracking()
            .Where(t => t.TenantId == tenantId
                     && !t.IsCompleted
                     && t.DueDate.HasValue
                     && t.DueDate.Value < now)
            .OrderBy(t => t.DueDate)
            .Select(t => new TaskSummaryDto(
                t.Id,
                t.Title,
                t.Priority,
                t.DueDate,
                t.IsCompleted,
                t.AssignedToUserId,
                t.LeadId,
                t.DealId,
                t.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}