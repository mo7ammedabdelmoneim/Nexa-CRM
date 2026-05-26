using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Notifications.Commands.MarkAsRead;

public class MarkAsReadCommandHandler
    : IRequestHandler<MarkAsReadCommand, Result>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkAsReadCommandHandler(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        MarkAsReadCommand command,
        CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository
            .GetByIdAsync(command.NotificationId, cancellationToken);

        if (notification is null)
            return Result.Failure("Notification not found.");

        if (notification.UserId != command.UserId)
            return Result.Failure("Access denied.");

        notification.MarkAsRead();

        _notificationRepository.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}