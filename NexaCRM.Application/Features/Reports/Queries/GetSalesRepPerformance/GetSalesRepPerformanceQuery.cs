using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Reports.Queries.GetSalesRepPerformance;

public record GetSalesRepPerformanceQuery(
    Guid TenantId) : IRequest<Result<IReadOnlyList<SalesRepPerformanceDto>>>;