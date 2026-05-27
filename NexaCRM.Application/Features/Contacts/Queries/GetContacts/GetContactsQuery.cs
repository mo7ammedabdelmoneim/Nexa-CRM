using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Contacts.Queries.GetContacts;

public record GetContactsQuery(
    Guid TenantId,
    string? Search = null,
    string? Company = null,
    Guid? LeadId = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedResult<ContactSummaryDto>>>;