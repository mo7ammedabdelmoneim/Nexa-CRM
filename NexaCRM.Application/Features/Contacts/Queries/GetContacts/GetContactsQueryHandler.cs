using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Contacts.Queries.GetContacts;

public class GetContactsQueryHandler
    : IRequestHandler<GetContactsQuery, Result<PaginatedResult<ContactSummaryDto>>>
{
    private readonly IContactQueryRepository _contactQueryRepository;

    public GetContactsQueryHandler(IContactQueryRepository contactQueryRepository)
        => _contactQueryRepository = contactQueryRepository;

    public async Task<Result<PaginatedResult<ContactSummaryDto>>> Handle(
        GetContactsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _contactQueryRepository.GetPagedAsync(
            query.TenantId,
            query.Search,
            query.Company,
            query.LeadId,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result<PaginatedResult<ContactSummaryDto>>.Success(result);
    }
}