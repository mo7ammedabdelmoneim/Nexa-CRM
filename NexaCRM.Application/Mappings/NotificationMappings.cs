using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Notifications;

namespace NexaCRM.Application.Mappings;

public static class NotificationMappings
{
    public static NotificationDto ToDto(this Notification notification) => new(
        notification.Id,
        notification.Message,
        notification.Type,
        notification.IsRead,
        notification.EntityId,
        notification.EntityType,
        notification.CreatedAt);
}