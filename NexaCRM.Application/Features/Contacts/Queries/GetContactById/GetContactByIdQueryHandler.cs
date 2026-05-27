using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;
using NexaCRM.Application.Mappings;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Contacts.Queries.GetContactById;

public class GetContactByIdQueryHandler
    : IRequestHandler<GetContactByIdQuery, Result<ContactDto>>
{
    private readonly IContactRepository _contactRepository;

    public GetContactByIdQueryHandler(IContactRepository contactRepository)
        => _contactRepository = contactRepository;

    public async Task<Result<ContactDto>> Handle(
        GetContactByIdQuery query,
        CancellationToken cancellationToken)
    {
        var contact = await _contactRepository
            .GetByIdAsync(query.ContactId, cancellationToken);

        return contact is null
            ? Result<ContactDto>.Failure("Contact not found.")
            : Result<ContactDto>.Success(contact.ToDto());
    }
}