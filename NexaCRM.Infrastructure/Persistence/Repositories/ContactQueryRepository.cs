using Microsoft.EntityFrameworkCore;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class ContactQueryRepository : IContactQueryRepository
{
    private readonly AppDbContext _context;

    public ContactQueryRepository(AppDbContext context)
        => _context = context;

    public async Task<PaginatedResult<ContactSummaryDto>> GetPagedAsync(
        Guid tenantId,
        string? search,
        string? company,
        Guid? leadId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Contacts
            .AsNoTracking()
            .Where(c => c.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.FirstName.Contains(search) ||
                c.LastName.Contains(search) ||
                c.ContactInfo.Email.Contains(search));

        if (!string.IsNullOrWhiteSpace(company))
            query = query.Where(c => c.Company != null &&
                c.Company.Contains(company));

        if (leadId.HasValue)
            query = query.Where(c => c.LeadId == leadId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ContactSummaryDto(
                c.Id,
                c.FirstName + " " + c.LastName,
                c.ContactInfo.Email,
                c.Company,
                c.JobTitle,
                c.LeadId,
                c.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ContactSummaryDto>(
            items, totalCount, page, pageSize);
    }
}