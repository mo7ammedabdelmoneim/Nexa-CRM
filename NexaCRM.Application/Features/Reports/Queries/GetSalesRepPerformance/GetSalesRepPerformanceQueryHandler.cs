using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Reports.Queries.GetSalesRepPerformance;

public class GetSalesRepPerformanceQueryHandler
    : IRequestHandler<GetSalesRepPerformanceQuery, Result<IReadOnlyList<SalesRepPerformanceDto>>>
{
    private readonly IReportRepository _reportRepository;

    public GetSalesRepPerformanceQueryHandler(IReportRepository reportRepository)
        => _reportRepository = reportRepository;

    public async Task<Result<IReadOnlyList<SalesRepPerformanceDto>>> Handle(
        GetSalesRepPerformanceQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _reportRepository.GetSalesRepPerformanceAsync(
            query.TenantId,
            cancellationToken);

        return Result<IReadOnlyList<SalesRepPerformanceDto>>.Success(result);
    }
}