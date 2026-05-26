using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Features.Notifications.Commands.MarkAllAsRead;
using NexaCRM.Application.Features.Notifications.Commands.MarkAsRead;
using NexaCRM.Application.Features.Notifications.Queries.GetNotifications;
using NexaCRM.Application.Features.Notifications.Queries.GetUnreadCount;

namespace NexaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly ISender _sender;

    private static readonly Guid _userId =
        Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7");

    public NotificationsController(ISender sender)
        => _sender = sender;

    // GET api/notifications
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isRead,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetNotificationsQuery(_userId, isRead, page, pageSize),
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
            new GetUnreadCountQuery(_userId), cancellationToken);

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
            new MarkAsReadCommand(id, _userId), cancellationToken);

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
            new MarkAllAsReadCommand(_userId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }
}