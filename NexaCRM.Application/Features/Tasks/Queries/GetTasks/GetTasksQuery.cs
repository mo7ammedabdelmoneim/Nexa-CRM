using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Tasks.Queries.GetTasks;

public record GetTasksQuery(
    Guid TenantId,
    Guid? AssignedToUserId = null,
    bool? IsCompleted = null,
    string? Priority = null,
    DateTime? DueBefore = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedResult<TaskSummaryDto>>>;