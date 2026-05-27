using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Contracts;

public interface IContactQueryRepository
{
    Task<PaginatedResult<ContactSummaryDto>> GetPagedAsync(
        Guid tenantId,
        string? search,
        string? company,
        Guid? leadId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}