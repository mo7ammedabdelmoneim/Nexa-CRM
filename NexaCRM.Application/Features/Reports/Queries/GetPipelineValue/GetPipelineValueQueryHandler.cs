using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Reports.Queries.GetPipelineValue;

public class GetPipelineValueQueryHandler
    : IRequestHandler<GetPipelineValueQuery, Result<PipelineValueReportDto>>
{
    private readonly IReportRepository _reportRepository;

    public GetPipelineValueQueryHandler(IReportRepository reportRepository)
        => _reportRepository = reportRepository;

    public async Task<Result<PipelineValueReportDto>> Handle(
        GetPipelineValueQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _reportRepository.GetPipelineValueAsync(
            query.TenantId,
            query.AssignedToUserId,
            cancellationToken);

        return Result<PipelineValueReportDto>.Success(result);
    }
}