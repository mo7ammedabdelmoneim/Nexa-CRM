using NexaCRM.Domain.Aggregates.Contacts;

namespace NexaCRM.Domain.Repositories;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAndTenantAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Contact contact, CancellationToken cancellationToken = default);
    void Update(Contact contact);
    void Delete(Contact contact);
}