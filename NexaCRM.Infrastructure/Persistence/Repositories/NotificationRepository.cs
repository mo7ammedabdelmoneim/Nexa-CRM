using Microsoft.EntityFrameworkCore;
using NexaCRM.Domain.Aggregates.Notifications;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
        => _context = context;

    public async Task AddAsync(
        Notification notification,
        CancellationToken cancellationToken = default)
        => await _context.Notifications.AddAsync(notification, cancellationToken);

    public async Task<Notification?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public void Update(Notification notification)
        => _context.Notifications.Update(notification);
}