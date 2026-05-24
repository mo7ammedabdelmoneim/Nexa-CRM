using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;
using NexaCRM.Application.Mappings;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Leads.Queries.GetLeadById;

public class GetLeadByIdQueryHandler
    : IRequestHandler<GetLeadByIdQuery, Result<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetLeadByIdQueryHandler(ILeadRepository leadRepository)
        => _leadRepository = leadRepository;

    public async Task<Result<LeadDto>> Handle(
        GetLeadByIdQuery query,
        CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(
            query.LeadId, cancellationToken);

        if (lead is null)
            return Result<LeadDto>.Failure("Lead not found.");

        return Result<LeadDto>.Success(lead.ToDto());
    }
}