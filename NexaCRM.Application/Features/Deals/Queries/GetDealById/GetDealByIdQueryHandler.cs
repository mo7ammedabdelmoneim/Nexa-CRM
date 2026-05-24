using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;
using NexaCRM.Application.Mappings;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Deals.Queries.GetDealById;

public class GetDealByIdQueryHandler
    : IRequestHandler<GetDealByIdQuery, Result<DealDto>>
{
    private readonly IDealRepository _dealRepository;

    public GetDealByIdQueryHandler(IDealRepository dealRepository)
        => _dealRepository = dealRepository;

    public async Task<Result<DealDto>> Handle(
        GetDealByIdQuery query,
        CancellationToken cancellationToken)
    {
        var deal = await _dealRepository.GetByIdAsync(
            query.DealId, cancellationToken);

        return deal is null
            ? Result<DealDto>.Failure("Deal not found.")
            : Result<DealDto>.Success(deal.ToDto());
    }
}

