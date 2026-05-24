using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Deals.Queries.GetDeals;

public record GetDealsQuery(
    Guid TenantId,
    string? Stage = null,
    Guid? AssignedToUserId = null,
    decimal? MinValue = null,
    decimal? MaxValue = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedResult<DealSummaryDto>>>;