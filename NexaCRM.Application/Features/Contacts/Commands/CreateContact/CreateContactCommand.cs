using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Contacts.Commands.CreateContact;

public record CreateContactCommand(
    string FirstName,
    string LastName,
    string Email,
    Guid TenantId,
    Guid CreatedByUserId,
    string? Phone = null,
    string? Company = null,
    string? JobTitle = null,
    string? LinkedIn = null,
    string? Address = null,
    Guid? LeadId = null) : IRequest<Result<ContactDto>>;