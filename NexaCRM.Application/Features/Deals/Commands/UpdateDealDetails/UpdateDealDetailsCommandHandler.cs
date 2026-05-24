using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Domain.Exceptions;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Deals.Commands.UpdateDealDetails;

public class UpdateDealDetailsCommandHandler
    : IRequestHandler<UpdateDealDetailsCommand, Result>
{
    private readonly IDealRepository _dealRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDealDetailsCommandHandler(
        IDealRepository dealRepository,
        IUnitOfWork unitOfWork)
    {
        _dealRepository = dealRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateDealDetailsCommand command,
        CancellationToken cancellationToken)
    {
        var deal = await _dealRepository.GetByIdAsync(
            command.DealId, cancellationToken);

        if (deal is null)
            return Result.Failure("Deal not found.");

        try
        {
            deal.UpdateDetails(
                command.Title,
                command.Amount,
                command.Currency,
                command.CloseDate);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        _dealRepository.Update(deal);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}