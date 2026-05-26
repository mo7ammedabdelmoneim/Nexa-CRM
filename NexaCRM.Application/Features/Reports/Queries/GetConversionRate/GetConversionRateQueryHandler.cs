using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Reports.Queries.GetConversionRate;

public class GetConversionRateQueryHandler
    : IRequestHandler<GetConversionRateQuery, Result<ConversionRateDto>>
{
    private readonly IReportRepository _reportRepository;

    public GetConversionRateQueryHandler(IReportRepository reportRepository)
        => _reportRepository = reportRepository;

    public async Task<Result<ConversionRateDto>> Handle(
        GetConversionRateQuery query,
        CancellationToken cancellationToken)
    {
        if (query.From > query.To)
            return Result<ConversionRateDto>.Failure(
                "From date cannot be greater than To date.");

        var result = await _reportRepository.GetConversionRateAsync(
            query.TenantId,
            query.From,
            query.To,
            query.AssignedToUserId,
            cancellationToken);

        return Result<ConversionRateDto>.Success(result);
    }
}