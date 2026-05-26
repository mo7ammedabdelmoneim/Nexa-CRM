using Microsoft.AspNetCore.SignalR;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Aggregates.Notifications;
using NexaCRM.Domain.Repositories;
using NexaCRM.Infrastructure.Hubs;

namespace NexaCRM.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        IHubContext<NotificationHub> hubContext)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    public async Task SendAsync(
        Guid userId,
        string message,
        string type,
        Guid? entityId = null,
        string? entityType = null,
        CancellationToken cancellationToken = default)
    {
        // Persist in DB
        var notification = Notification.Create(
            userId, message, type, entityId, entityType);

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Push real-time via SignalR
        await _hubContext.Clients
            .Group($"user-{userId}")
            .SendAsync("notification.new", new
            {
                id = notification.Id,
                message = notification.Message,
                type = notification.Type,
                entityId = notification.EntityId,
                entityType = notification.EntityType,
                createdAt = notification.CreatedAt,
            }, cancellationToken);
    }
}