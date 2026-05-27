using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Contacts.Commands.LinkContactToLead;

public class LinkContactToLeadCommandHandler
    : IRequestHandler<LinkContactToLeadCommand, Result>
{
    private readonly IContactRepository _contactRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LinkContactToLeadCommandHandler(
        IContactRepository contactRepository,
        ILeadRepository leadRepository,
        IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _leadRepository = leadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        LinkContactToLeadCommand command,
        CancellationToken cancellationToken)
    {
        var contact = await _contactRepository
            .GetByIdAsync(command.ContactId, cancellationToken);

        if (contact is null)
            return Result.Failure("Contact not found.");

        var lead = await _leadRepository
            .GetByIdAsync(command.LeadId, cancellationToken);

        if (lead is null)
            return Result.Failure("Lead not found.");

        contact.LinkToLead(command.LeadId);

        _contactRepository.Update(contact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}