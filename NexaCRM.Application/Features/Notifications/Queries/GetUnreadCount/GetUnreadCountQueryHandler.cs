using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;

namespace NexaCRM.Application.Features.Notifications.Queries.GetUnreadCount;

public class GetUnreadCountQueryHandler
    : IRequestHandler<GetUnreadCountQuery, Result<int>>
{
    private readonly INotificationQueryRepository _notificationQueryRepository;

    public GetUnreadCountQueryHandler(
        INotificationQueryRepository notificationQueryRepository)
        => _notificationQueryRepository = notificationQueryRepository;

    public async Task<Result<int>> Handle(
        GetUnreadCountQuery query,
        CancellationToken cancellationToken)
    {
        var count = await _notificationQueryRepository
            .GetUnreadCountAsync(query.UserId, cancellationToken);

        return Result<int>.Success(count);
    }
}