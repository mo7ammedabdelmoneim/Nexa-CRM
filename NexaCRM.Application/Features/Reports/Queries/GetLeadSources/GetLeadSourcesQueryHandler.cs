using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Reports.Queries.GetLeadSources;

public class GetLeadSourcesQueryHandler
    : IRequestHandler<GetLeadSourcesQuery, Result<IReadOnlyList<LeadSourceBreakdownDto>>>
{
    private readonly IReportRepository _reportRepository;

    public GetLeadSourcesQueryHandler(IReportRepository reportRepository)
        => _reportRepository = reportRepository;

    public async Task<Result<IReadOnlyList<LeadSourceBreakdownDto>>> Handle(
        GetLeadSourcesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _reportRepository.GetLeadSourceBreakdownAsync(
            query.TenantId,
            query.From,
            query.To,
            cancellationToken);

        return Result<IReadOnlyList<LeadSourceBreakdownDto>>.Success(result);
    }
}