using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Leads.Queries.GetLeads;

public class GetLeadsQueryHandler
    : IRequestHandler<GetLeadsQuery, Result<PaginatedResult<LeadSummaryDto>>>
{
    private readonly ILeadQueryRepository _leadQueryRepository;

    public GetLeadsQueryHandler(ILeadQueryRepository leadQueryRepository)
        => _leadQueryRepository = leadQueryRepository;

    public async Task<Result<PaginatedResult<LeadSummaryDto>>> Handle(
        GetLeadsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _leadQueryRepository.GetPagedAsync(
            query.TenantId,
            query.Status,
            query.AssignedToUserId,
            query.Source,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result<PaginatedResult<LeadSummaryDto>>.Success(result);
    }
}