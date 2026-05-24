using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Contracts;

public interface IDealQueryRepository
{
    Task<IReadOnlyList<PipelineStageDto>> GetPipelineOverviewAsync(
        Guid tenantId,
        Guid? assignedToUserId,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<DealSummaryDto>> GetPagedAsync(
        Guid tenantId,
        string? stage,
        Guid? assignedToUserId,
        decimal? minValue,
        decimal? maxValue,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}