using Microsoft.EntityFrameworkCore;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Deals;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class DealQueryRepository : IDealQueryRepository
{
    private readonly AppDbContext _context;

    public DealQueryRepository(AppDbContext context)
        => _context = context;

    public async Task<IReadOnlyList<PipelineStageDto>> GetPipelineOverviewAsync(
        Guid tenantId,
        Guid? assignedToUserId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Deals
            .AsNoTracking()
            .Where(d => d.TenantId == tenantId);

        if (assignedToUserId.HasValue)
            query = query.Where(d => d.AssignedToUserId == assignedToUserId);

        var stages = await query
            .GroupBy(d => d.Stage)
            .Select(g => new PipelineStageDto(
                g.Key,
                g.Count(),
                g.Sum(d => d.Value.Amount),
                "USD"))
            .ToListAsync(cancellationToken);

        var allStages = DealStage.All
            .Select(stage => stages.FirstOrDefault(s => s.Stage == stage)
                ?? new PipelineStageDto(stage, 0, 0, "USD"))
            .ToList();

        return allStages;
    }

    public async Task<PaginatedResult<DealSummaryDto>> GetPagedAsync(
        Guid tenantId,
        string? stage,
        Guid? assignedToUserId,
        decimal? minValue,
        decimal? maxValue,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Deals
            .AsNoTracking()
            .Where(d => d.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(stage))
            query = query.Where(d => d.Stage == stage);

        if (assignedToUserId.HasValue)
            query = query.Where(d => d.AssignedToUserId == assignedToUserId);

        if (minValue.HasValue)
            query = query.Where(d => d.Value.Amount >= minValue.Value);

        if (maxValue.HasValue)
            query = query.Where(d => d.Value.Amount <= maxValue.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DealSummaryDto(
                d.Id,
                d.Title,
                d.Stage,
                d.Value.Amount,
                d.Value.Currency,
                d.CloseDate,
                d.AssignedToUserId,
                d.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<DealSummaryDto>(items, totalCount, page, pageSize);
    }
}