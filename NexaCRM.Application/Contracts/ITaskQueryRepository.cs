using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Contracts;

public interface ITaskQueryRepository
{
    Task<PaginatedResult<TaskSummaryDto>> GetPagedAsync(
        Guid tenantId,
        Guid? assignedToUserId,
        bool? isCompleted,
        string? priority,
        DateTime? dueBefore,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskSummaryDto>> GetOverdueAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}