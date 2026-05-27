using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Contacts.Commands.DeleteContact;

public class DeleteContactCommandHandler
    : IRequestHandler<DeleteContactCommand, Result>
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContactCommandHandler(
        IContactRepository contactRepository,
        IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteContactCommand command,
        CancellationToken cancellationToken)
    {
        var contact = await _contactRepository
            .GetByIdAsync(command.ContactId, cancellationToken);

        if (contact is null)
            return Result.Failure("Contact not found.");

        contact.SoftDelete();
        _contactRepository.Update(contact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}