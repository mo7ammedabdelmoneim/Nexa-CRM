using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.Features.Notifications.Commands.MarkAllAsRead;
using NexaCRM.Application.Features.Notifications.Commands.MarkAsRead;
using NexaCRM.Application.Features.Notifications.Queries.GetNotifications;
using NexaCRM.Application.Features.Notifications.Queries.GetUnreadCount;

namespace NexaCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public NotificationsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    // GET api/notifications
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isRead,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetNotificationsQuery(_currentUser.UserId, isRead, page, pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/notifications/unread-count
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetUnreadCountQuery(_currentUser.UserId), cancellationToken);

        return result.IsSuccess
            ? Ok(new { count = result.Value })
            : BadRequest(new { error = result.Error });
    }

    // PATCH api/notifications/{id}/read
    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new MarkAsReadCommand(id, _currentUser.UserId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    // PATCH api/notifications/read-all
    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new MarkAllAsReadCommand(_currentUser.UserId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }
}