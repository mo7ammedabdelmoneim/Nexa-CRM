using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Contracts;

public interface ILeadQueryRepository
{
    Task<PaginatedResult<LeadSummaryDto>> GetPagedAsync(
        Guid tenantId,
        string? status,
        Guid? assignedToUserId,
        string? source,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}