using Microsoft.EntityFrameworkCore;
using NexaCRM.Domain.Aggregates.Contacts;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;

    public ContactRepository(AppDbContext context)
        => _context = context;

    public async Task<Contact?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<bool> ExistsByEmailAndTenantAsync(
        string email,
        Guid tenantId,
        CancellationToken cancellationToken = default)
        => await _context.Contacts
            .AnyAsync(c =>
                c.ContactInfo.Email == email.ToLower() &&
                c.TenantId == tenantId,
                cancellationToken);

    public async Task AddAsync(
        Contact contact,
        CancellationToken cancellationToken = default)
        => await _context.Contacts.AddAsync(contact, cancellationToken);

    public void Update(Contact contact)
        => _context.Contacts.Update(contact);

    public void Delete(Contact contact)
        => _context.Contacts.Update(contact);
}