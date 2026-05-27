using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Contacts.Queries.GetContactById;

public record GetContactByIdQuery(Guid ContactId)
    : IRequest<Result<ContactDto>>;