using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Domain.Exceptions;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Leads.Commands.AssignLead;

public class AssignLeadCommandHandler
    : IRequestHandler<AssignLeadCommand, Result>
{
    private readonly ILeadRepository _leadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignLeadCommandHandler(
        ILeadRepository leadRepository,
        IUnitOfWork unitOfWork)
    {
        _leadRepository = leadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        AssignLeadCommand command,
        CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(
            command.LeadId, cancellationToken);

        if (lead is null)
            return Result.Failure("Lead not found.");

        try
        {
            lead.Assign(command.AssignedToUserId);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        _leadRepository.Update(lead);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}