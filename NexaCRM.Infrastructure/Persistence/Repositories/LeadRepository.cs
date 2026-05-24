using Microsoft.EntityFrameworkCore;
using NexaCRM.Domain.Aggregates.Leads;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class LeadRepository : ILeadRepository
{
    private readonly AppDbContext _context;

    public LeadRepository(AppDbContext context)
        => _context = context;

    public async Task<Lead?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await _context.Leads
            .Include(l => l.Notes)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<bool> ExistsByEmailAndTenantAsync(
        string email,
        Guid tenantId,
        CancellationToken cancellationToken = default)
        => await _context.Leads
            .AnyAsync(l => l.ContactInfo.Email == email
                        && l.TenantId == tenantId,
                      cancellationToken);

    public async Task AddAsync(
        Lead lead,
        CancellationToken cancellationToken = default)
        => await _context.Leads.AddAsync(lead, cancellationToken);

    public void Update(Lead lead)
        => _context.Leads.Update(lead);

    public void Delete(Lead lead)
        => _context.Leads.Update(lead); // just update IsDeleted
}

using Microsoft.EntityFrameworkCore;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;
