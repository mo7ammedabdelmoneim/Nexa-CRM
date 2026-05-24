using Microsoft.EntityFrameworkCore;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class LeadQueryRepository : ILeadQueryRepository
{
    private readonly AppDbContext _context;

    public LeadQueryRepository(AppDbContext context)
        => _context = context;

    public async Task<PaginatedResult<LeadSummaryDto>> GetPagedAsync(
        Guid tenantId,
        string? status,
        Guid? assignedToUserId,
        string? source,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Leads
            .AsNoTracking()
            .Where(l => l.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(l => l.Status == status);

        if (assignedToUserId.HasValue)
            query = query.Where(l => l.AssignedToUserId == assignedToUserId);

        if (!string.IsNullOrWhiteSpace(source))
            query = query.Where(l => l.Source == source);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LeadSummaryDto(
                l.Id,
                l.Name,
                l.ContactInfo.Email,
                l.Status,
                l.Source,
                l.AssignedToUserId,
                l.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<LeadSummaryDto>(items, totalCount, page, pageSize);
    }
}