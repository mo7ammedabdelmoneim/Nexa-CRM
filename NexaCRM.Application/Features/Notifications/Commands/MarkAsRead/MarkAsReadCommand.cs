using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Notifications.Commands.MarkAsRead;

public record MarkAsReadCommand(
    Guid NotificationId,
    Guid UserId) : IRequest<Result>;