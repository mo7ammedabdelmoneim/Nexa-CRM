using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Domain.Exceptions;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Deals.Commands.AssignDeal;

public class AssignDealCommandHandler
    : IRequestHandler<AssignDealCommand, Result>
{
    private readonly IDealRepository _dealRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignDealCommandHandler(
        IDealRepository dealRepository,
        IUnitOfWork unitOfWork)
    {
        _dealRepository = dealRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        AssignDealCommand command,
        CancellationToken cancellationToken)
    {
        var deal = await _dealRepository.GetByIdAsync(
            command.DealId, cancellationToken);

        if (deal is null)
            return Result.Failure("Deal not found.");

        try
        {
            deal.Assign(command.AssignedToUserId);
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