using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Reports.Queries.GetActivitySummary;

public record GetActivitySummaryQuery(
    Guid TenantId,
    DateTime From,
    DateTime To) : IRequest<Result<IReadOnlyList<ActivitySummaryDto>>>;