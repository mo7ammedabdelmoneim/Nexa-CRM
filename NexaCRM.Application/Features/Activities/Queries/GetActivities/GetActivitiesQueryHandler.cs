using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Activities.Queries.GetActivities;

public class GetActivitiesQueryHandler
    : IRequestHandler<GetActivitiesQuery, Result<PaginatedResult<ActivityDto>>>
{
    private readonly IActivityQueryRepository _activityQueryRepository;

    public GetActivitiesQueryHandler(IActivityQueryRepository activityQueryRepository)
        => _activityQueryRepository = activityQueryRepository;

    public async Task<Result<PaginatedResult<ActivityDto>>> Handle(
        GetActivitiesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _activityQueryRepository.GetPagedAsync(
            query.TenantId,
            query.LeadId,
            query.DealId,
            query.Type,
            query.From,
            query.To,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result<PaginatedResult<ActivityDto>>.Success(result);
    }
}