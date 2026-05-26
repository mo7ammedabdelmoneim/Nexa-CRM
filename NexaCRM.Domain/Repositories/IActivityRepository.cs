using NexaCRM.Domain.Aggregates.Activities;

namespace NexaCRM.Domain.Repositories;

public interface IActivityRepository
{
    Task<Activity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Activity activity, CancellationToken cancellationToken = default);
}