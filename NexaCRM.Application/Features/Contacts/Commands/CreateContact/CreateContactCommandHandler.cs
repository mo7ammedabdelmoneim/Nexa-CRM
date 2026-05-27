using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;
using NexaCRM.Application.Mappings;
using NexaCRM.Domain.Aggregates.Contacts;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Contacts.Commands.CreateContact;

public class CreateContactCommandHandler
    : IRequestHandler<CreateContactCommand, Result<ContactDto>>
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContactCommandHandler(
        IContactRepository contactRepository,
        IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ContactDto>> Handle(
        CreateContactCommand command,
        CancellationToken cancellationToken)
    {
        var emailExists = await _contactRepository
            .ExistsByEmailAndTenantAsync(
                command.Email, command.TenantId, cancellationToken);

        if (emailExists)
            return Result<ContactDto>.Failure(
                "A contact with this email already exists.");

        var contact = Contact.Create(
            command.FirstName,
            command.LastName,
            command.Email,
            command.TenantId,
            command.CreatedByUserId,
            command.Phone,
            command.Company,
            command.JobTitle,
            command.LinkedIn,
            command.Address,
            command.LeadId);

        await _contactRepository.AddAsync(contact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ContactDto>.Success(contact.ToDto());
    }
}