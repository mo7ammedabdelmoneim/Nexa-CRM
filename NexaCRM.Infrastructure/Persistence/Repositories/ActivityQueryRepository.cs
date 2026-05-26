using Microsoft.EntityFrameworkCore;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class ActivityQueryRepository : IActivityQueryRepository
{
    private readonly AppDbContext _context;

    public ActivityQueryRepository(AppDbContext context)
        => _context = context;

    public async Task<PaginatedResult<ActivityDto>> GetPagedAsync(
        Guid tenantId,
        Guid? leadId,
        Guid? dealId,
        string? type,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Activities
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId);

        if (leadId.HasValue)
            query = query.Where(a => a.LeadId == leadId);

        if (dealId.HasValue)
            query = query.Where(a => a.DealId == dealId);

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(a => a.Type == type);

        if (from.HasValue)
            query = query.Where(a => a.OccurredAt >= from.Value);

        if (to.HasValue)
            query = query.Where(a => a.OccurredAt <= to.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.OccurredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ActivityDto(
                a.Id,
                a.Type,
                a.Description,
                a.OccurredAt,
                a.LeadId,
                a.DealId,
                a.CreatedByUserId!.Value,
                a.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ActivityDto>(items, totalCount, page, pageSize);
    }
}