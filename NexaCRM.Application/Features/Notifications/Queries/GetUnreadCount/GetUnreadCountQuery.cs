using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Notifications.Queries.GetUnreadCount;

public record GetUnreadCountQuery(Guid UserId) : IRequest<Result<int>>;