using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(
    Guid UserId,
    bool? IsRead = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedResult<NotificationDto>>>;