using Microsoft.EntityFrameworkCore;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Deals;
using NexaCRM.Domain.Aggregates.Leads;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
        => _context = context;

    // Conversion Rate 
    public async Task<ConversionRateDto> GetConversionRateAsync(
        Guid tenantId,
        DateTime from,
        DateTime to,
        Guid? assignedToUserId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Leads
            .AsNoTracking()
            .Where(l => l.TenantId == tenantId
                     && l.CreatedAt >= from
                     && l.CreatedAt <= to);

        if (assignedToUserId.HasValue)
            query = query.Where(l => l.AssignedToUserId == assignedToUserId);

        var totalLeads = await query.CountAsync(cancellationToken);

        var wonLeads = await query
            .CountAsync(l => l.Status == LeadStatus.Won, cancellationToken);

        var conversionRate = totalLeads == 0
            ? 0
            : Math.Round((double)wonLeads / totalLeads * 100, 2);

        return new ConversionRateDto(
            totalLeads,
            wonLeads,
            conversionRate,
            from,
            to);
    }

    // Pipeline Value 
    public async Task<PipelineValueReportDto> GetPipelineValueAsync(
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

        var totalValue = allStages.Sum(s => s.TotalValue);

        return new PipelineValueReportDto(allStages, totalValue, "USD");
    }

    // Sales Rep Performance 
    public async Task<IReadOnlyList<SalesRepPerformanceDto>> GetSalesRepPerformanceAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var leadStats = await _context.Leads
            .AsNoTracking()
            .Where(l => l.TenantId == tenantId && l.AssignedToUserId.HasValue)
            .GroupBy(l => l.AssignedToUserId!.Value)
            .Select(g => new
            {
                UserId = g.Key,
                LeadsHandled = g.Count(),
            })
            .ToListAsync(cancellationToken);

        var dealStats = await _context.Deals
            .AsNoTracking()
            .Where(d => d.TenantId == tenantId
                     && d.AssignedToUserId.HasValue
                     && d.Stage == DealStage.Won)
            .GroupBy(d => d.AssignedToUserId!.Value)
            .Select(g => new
            {
                UserId = g.Key,
                DealsWon = g.Count(),
                TotalValue = g.Sum(d => d.Value.Amount),
                AvgCloseDays = g.Average(d =>
                    d.UpdatedAt.HasValue
                        ? EF.Functions.DateDiffDay(d.CreatedAt, d.UpdatedAt.Value)
                        : 0),
            })
            .ToListAsync(cancellationToken);

        var result = leadStats.Select(ls =>
        {
            var ds = dealStats.FirstOrDefault(d => d.UserId == ls.UserId);
            return new SalesRepPerformanceDto(
                ls.UserId,
                ls.LeadsHandled,
                ds?.DealsWon ?? 0,
                ds?.TotalValue ?? 0,
                Math.Round(ds?.AvgCloseDays ?? 0, 1));
        }).ToList();

        return result;
    }

    // Lead Source Breakdown 
    public async Task<IReadOnlyList<LeadSourceBreakdownDto>> GetLeadSourceBreakdownAsync(
        Guid tenantId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var total = await _context.Leads
            .AsNoTracking()
            .CountAsync(l => l.TenantId == tenantId
                          && l.CreatedAt >= from
                          && l.CreatedAt <= to,
                        cancellationToken);

        if (total == 0)
            return [];

        var breakdown = await _context.Leads
            .AsNoTracking()
            .Where(l => l.TenantId == tenantId
                     && l.CreatedAt >= from
                     && l.CreatedAt <= to)
            .GroupBy(l => l.Source)
            .Select(g => new
            {
                Source = g.Key,
                Count = g.Count(),
            })
            .ToListAsync(cancellationToken);

        return breakdown
            .Select(b => new LeadSourceBreakdownDto(
                b.Source,
                b.Count,
                Math.Round((double)b.Count / total * 100, 2)))
            .OrderByDescending(b => b.Count)
            .ToList();
    }

    // Activity Summary 
    public async Task<IReadOnlyList<ActivitySummaryDto>> GetActivitySummaryAsync(
        Guid tenantId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        return await _context.Activities
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId
                     && a.OccurredAt >= from
                     && a.OccurredAt <= to)
            .GroupBy(a => a.Type)
            .Select(g => new ActivitySummaryDto(g.Key, g.Count()))
            .OrderByDescending(a => a.Count)
            .ToListAsync(cancellationToken);
    }
}