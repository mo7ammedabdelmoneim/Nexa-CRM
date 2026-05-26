using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Contracts;

public interface IActivityQueryRepository
{
    Task<PaginatedResult<ActivityDto>> GetPagedAsync(
        Guid tenantId,
        Guid? leadId,
        Guid? dealId,
        string? type,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}