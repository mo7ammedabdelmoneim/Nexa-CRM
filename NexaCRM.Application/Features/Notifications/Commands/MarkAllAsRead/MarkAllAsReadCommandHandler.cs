using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Notifications.Commands.MarkAllAsRead;

public class MarkAllAsReadCommandHandler
    : IRequestHandler<MarkAllAsReadCommand, Result>
{
    private readonly INotificationQueryRepository _notificationQueryRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllAsReadCommandHandler(
        INotificationQueryRepository notificationQueryRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationQueryRepository = notificationQueryRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        MarkAllAsReadCommand command,
        CancellationToken cancellationToken)
    {
        await _notificationQueryRepository
            .MarkAllAsReadAsync(command.UserId, cancellationToken);

        return Result.Success();
    }
}