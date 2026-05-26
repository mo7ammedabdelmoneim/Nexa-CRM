using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Features.Activities.Commands.LogActivity;
using NexaCRM.Application.Features.Activities.Queries.GetActivities;

namespace NexaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly ISender _sender;

    private static readonly Guid _tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    private static readonly Guid _userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7");

    public ActivitiesController(ISender sender)
        => _sender = sender;

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
            _tenantId,
            _userId,
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
            _tenantId, leadId, dealId, type, from, to, page, pageSize),
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