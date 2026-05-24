using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;
using NexaCRM.Application.Mappings;
using NexaCRM.Domain.Aggregates.Leads;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Leads.Commands.CreateLead;

public class CreateLeadCommandHandler
    : IRequestHandler<CreateLeadCommand, Result<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLeadCommandHandler(
        ILeadRepository leadRepository,
        IUnitOfWork unitOfWork)
    {
        _leadRepository = leadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LeadDto>> Handle(
        CreateLeadCommand command,
        CancellationToken cancellationToken)
    {
        var emailExists = await _leadRepository.ExistsByEmailAndTenantAsync(
            command.Email, command.TenantId, cancellationToken);

        if (emailExists)
            return Result<LeadDto>.Failure(
                "A lead with this email already exists.");

        var lead = Lead.Create(
            command.Name,
            command.Email,
            command.Source,
            command.TenantId,
            command.CreatedByUserId,
            command.Phone,
            command.AssignedToUserId);

        await _leadRepository.AddAsync(lead, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LeadDto>.Success(lead.ToDto());
    }
}