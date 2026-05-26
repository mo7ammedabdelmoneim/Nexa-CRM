using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Reports.Queries.GetActivitySummary;

public class GetActivitySummaryQueryHandler
    : IRequestHandler<GetActivitySummaryQuery, Result<IReadOnlyList<ActivitySummaryDto>>>
{
    private readonly IReportRepository _reportRepository;

    public GetActivitySummaryQueryHandler(IReportRepository reportRepository)
        => _reportRepository = reportRepository;

    public async Task<Result<IReadOnlyList<ActivitySummaryDto>>> Handle(
        GetActivitySummaryQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _reportRepository.GetActivitySummaryAsync(
            query.TenantId,
            query.From,
            query.To,
            cancellationToken);

        return Result<IReadOnlyList<ActivitySummaryDto>>.Success(result);
    }
}