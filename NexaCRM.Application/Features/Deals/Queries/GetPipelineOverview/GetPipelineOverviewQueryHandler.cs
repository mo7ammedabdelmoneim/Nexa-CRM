using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Deals.Queries.GetPipelineOverview;

public class GetPipelineOverviewQueryHandler
    : IRequestHandler<GetPipelineOverviewQuery, Result<IReadOnlyList<PipelineStageDto>>>
{
    private readonly IDealQueryRepository _dealQueryRepository;

    public GetPipelineOverviewQueryHandler(IDealQueryRepository dealQueryRepository)
        => _dealQueryRepository = dealQueryRepository;

    public async Task<Result<IReadOnlyList<PipelineStageDto>>> Handle(
        GetPipelineOverviewQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _dealQueryRepository.GetPipelineOverviewAsync(
            query.TenantId,
            query.AssignedToUserId,
            cancellationToken);

        return Result<IReadOnlyList<PipelineStageDto>>.Success(result);
    }
}