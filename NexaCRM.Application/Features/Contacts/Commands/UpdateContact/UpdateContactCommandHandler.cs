using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Domain.Exceptions;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Contacts.Commands.UpdateContact;

public class UpdateContactCommandHandler
    : IRequestHandler<UpdateContactCommand, Result>
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContactCommandHandler(
        IContactRepository contactRepository,
        IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateContactCommand command,
        CancellationToken cancellationToken)
    {
        var contact = await _contactRepository
            .GetByIdAsync(command.ContactId, cancellationToken);

        if (contact is null)
            return Result.Failure("Contact not found.");

        try
        {
            contact.UpdateDetails(
                command.FirstName,
                command.LastName,
                command.Email,
                command.Phone,
                command.Company,
                command.JobTitle,
                command.LinkedIn,
                command.Address);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        _contactRepository.Update(contact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}