using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Contacts.Commands.UpdateContact;

public record UpdateContactCommand(
    Guid ContactId,
    string FirstName,
    string LastName,
    string Email,
    string? Phone = null,
    string? Company = null,
    string? JobTitle = null,
    string? LinkedIn = null,
    string? Address = null) : IRequest<Result>;