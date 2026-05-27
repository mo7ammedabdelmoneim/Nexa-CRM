using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.Features.Activities.Commands.LogActivity;
using NexaCRM.Application.Features.Activities.Queries.GetActivities;

namespace NexaCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public ActivitiesController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    // POST api/activities
    [HttpPost]
    public async Task<IActionResult> Log(
        [FromBody] LogActivityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new LogActivityCommand(
            request.Type,
            request.Description,
            request.OccurredAt,
            _currentUser.TenantId,
            _currentUser.UserId,
            request.LeadId,
            request.DealId), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/activities
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? leadId,
        [FromQuery] Guid? dealId,
        [FromQuery] string? type,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetActivitiesQuery(
            _currentUser.TenantId, leadId, dealId, type, from, to, page, pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }
}

public record LogActivityRequest(
    string Type,
    string Description,
    DateTime OccurredAt,
    Guid? LeadId,
    Guid? DealId);