using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Notifications.Commands.MarkAllAsRead;

public record MarkAllAsReadCommand(Guid UserId) : IRequest<Result>;