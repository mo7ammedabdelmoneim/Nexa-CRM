using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler
    : IRequestHandler<GetTasksQuery, Result<PaginatedResult<TaskSummaryDto>>>
{
    private readonly ITaskQueryRepository _taskQueryRepository;

    public GetTasksQueryHandler(ITaskQueryRepository taskQueryRepository)
        => _taskQueryRepository = taskQueryRepository;

    public async Task<Result<PaginatedResult<TaskSummaryDto>>> Handle(
        GetTasksQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _taskQueryRepository.GetPagedAsync(
            query.TenantId,
            query.AssignedToUserId,
            query.IsCompleted,
            query.Priority,
            query.DueBefore,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result<PaginatedResult<TaskSummaryDto>>.Success(result);
    }
}