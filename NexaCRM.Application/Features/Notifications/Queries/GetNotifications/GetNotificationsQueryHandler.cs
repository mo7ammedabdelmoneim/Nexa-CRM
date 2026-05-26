using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler
    : IRequestHandler<GetNotificationsQuery, Result<PaginatedResult<NotificationDto>>>
{
    private readonly INotificationQueryRepository _notificationQueryRepository;

    public GetNotificationsQueryHandler(
        INotificationQueryRepository notificationQueryRepository)
        => _notificationQueryRepository = notificationQueryRepository;

    public async Task<Result<PaginatedResult<NotificationDto>>> Handle(
        GetNotificationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _notificationQueryRepository.GetPagedAsync(
            query.UserId,
            query.IsRead,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result<PaginatedResult<NotificationDto>>.Success(result);
    }
}