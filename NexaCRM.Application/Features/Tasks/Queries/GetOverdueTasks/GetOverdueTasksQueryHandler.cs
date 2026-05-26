using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Tasks.Queries.GetOverdueTasks;

public class GetOverdueTasksQueryHandler
    : IRequestHandler<GetOverdueTasksQuery, Result<IReadOnlyList<TaskSummaryDto>>>
{
    private readonly ITaskQueryRepository _taskQueryRepository;

    public GetOverdueTasksQueryHandler(ITaskQueryRepository taskQueryRepository)
        => _taskQueryRepository = taskQueryRepository;

    public async Task<Result<IReadOnlyList<TaskSummaryDto>>> Handle(
        GetOverdueTasksQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _taskQueryRepository.GetOverdueAsync(
            query.TenantId, cancellationToken);

        return Result<IReadOnlyList<TaskSummaryDto>>.Success(result);
    }
}