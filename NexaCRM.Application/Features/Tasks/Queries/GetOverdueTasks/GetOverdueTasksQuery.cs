using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Tasks.Queries.GetOverdueTasks;

public record GetOverdueTasksQuery(Guid TenantId)
    : IRequest<Result<IReadOnlyList<TaskSummaryDto>>>;