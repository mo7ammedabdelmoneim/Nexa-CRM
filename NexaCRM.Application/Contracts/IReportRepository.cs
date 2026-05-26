using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Contracts;

public interface IReportRepository
{
    Task<ConversionRateDto> GetConversionRateAsync(
        Guid tenantId,
        DateTime from,
        DateTime to,
        Guid? assignedToUserId,
        CancellationToken cancellationToken = default);

    Task<PipelineValueReportDto> GetPipelineValueAsync(
        Guid tenantId,
        Guid? assignedToUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SalesRepPerformanceDto>> GetSalesRepPerformanceAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeadSourceBreakdownDto>> GetLeadSourceBreakdownAsync(
        Guid tenantId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ActivitySummaryDto>> GetActivitySummaryAsync(
        Guid tenantId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}