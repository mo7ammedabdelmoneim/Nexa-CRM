using NexaCRM.Domain.Aggregates.Deals;

namespace NexaCRM.Domain.Repositories;

public interface IDealRepository
{
    Task<Deal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Deal deal, CancellationToken cancellationToken = default);
    void Update(Deal deal);
}