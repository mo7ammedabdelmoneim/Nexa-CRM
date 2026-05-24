using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Deals.Queries.GetDeals;

public class GetDealsQueryHandler
    : IRequestHandler<GetDealsQuery, Result<PaginatedResult<DealSummaryDto>>>
{
    private readonly IDealQueryRepository _dealQueryRepository;

    public GetDealsQueryHandler(IDealQueryRepository dealQueryRepository)
        => _dealQueryRepository = dealQueryRepository;

    public async Task<Result<PaginatedResult<DealSummaryDto>>> Handle(
        GetDealsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _dealQueryRepository.GetPagedAsync(
            query.TenantId,
            query.Stage,
            query.AssignedToUserId,
            query.MinValue,
            query.MaxValue,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result<PaginatedResult<DealSummaryDto>>.Success(result);
    }
}