using NexaCRM.Domain.Aggregates.Notifications;

namespace NexaCRM.Domain.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(Notification notification);
}