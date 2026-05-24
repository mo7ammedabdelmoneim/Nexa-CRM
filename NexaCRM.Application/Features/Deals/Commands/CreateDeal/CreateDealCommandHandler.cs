using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;
using NexaCRM.Application.Mappings;
using NexaCRM.Domain.Aggregates.Deals;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Deals.Commands.CreateDeal;

public class CreateDealCommandHandler
    : IRequestHandler<CreateDealCommand, Result<DealDto>>
{
    private readonly IDealRepository _dealRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDealCommandHandler(
        IDealRepository dealRepository,
        ILeadRepository leadRepository,
        IUnitOfWork unitOfWork)
    {
        _dealRepository = dealRepository;
        _leadRepository = leadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DealDto>> Handle(
        CreateDealCommand command,
        CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(
            command.LeadId, cancellationToken);

        if (lead is null)
            return Result<DealDto>.Failure("Lead not found.");

        var deal = Deal.Create(
            command.LeadId,
            command.Title,
            command.Amount,
            command.TenantId,
            command.CreatedByUserId,
            command.Currency,
            command.CloseDate,
            command.AssignedToUserId);

        lead.MarkAsConverted(deal.Id);

        await _dealRepository.AddAsync(deal, cancellationToken);
        _leadRepository.Update(lead);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DealDto>.Success(deal.ToDto());
    }
}