using NexaCRM.Domain.Aggregates.Tasks;

namespace NexaCRM.Domain.Repositories;

public interface ITaskRepository
{
    Task<CrmTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(CrmTask task, CancellationToken cancellationToken = default);
    void Update(CrmTask task);
    void Delete(CrmTask task);
}