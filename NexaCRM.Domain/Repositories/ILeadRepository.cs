using NexaCRM.Domain.Aggregates.Leads;

namespace NexaCRM.Domain.Repositories;

public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAndTenantAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Lead lead, CancellationToken cancellationToken = default);
    void Update(Lead lead);
    void Delete(Lead lead);
}