using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Features.Leads.Commands.AssignLead;
using NexaCRM.Application.Features.Leads.Commands.CreateLead;
using NexaCRM.Application.Features.Leads.Commands.DeleteLead;
using NexaCRM.Application.Features.Leads.Commands.UpdateLeadStatus;
using NexaCRM.Application.Features.Leads.Queries.GetLeadById;
using NexaCRM.Application.Features.Leads.Queries.GetLeads;

namespace NexaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly ISender _sender;

    // Hardcoded for now — will come from JWT later
    private static readonly Guid _tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    private static readonly Guid _userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7");

    public LeadsController(ISender sender)
        => _sender = sender;

    // POST api/leads
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLeadRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLeadCommand(
            request.Name,
            request.Email,
            request.Source,
            _tenantId,
            _userId,
            request.Phone,
            request.AssignedToUserId);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/leads
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] Guid? assignedToUserId,
        [FromQuery] string? source,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLeadsQuery(
            _tenantId, status, assignedToUserId, source, page, pageSize);

        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/leads/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetLeadByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    // PATCH api/leads/{id}/status
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateLeadStatusCommand(id, request.NewStatus), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    // PATCH api/leads/{id}/assign
    [HttpPatch("{id:guid}/assign")]
    public async Task<IActionResult> Assign(
        Guid id,
        [FromBody] AssignLeadRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new AssignLeadCommand(id, request.AssignedToUserId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    // DELETE api/leads/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new DeleteLeadCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }
}

// Request Models 
public record CreateLeadRequest(
    string Name,
    string Email,
    string Source,
    string? Phone,
    Guid? AssignedToUserId);

public record UpdateStatusRequest(string NewStatus);
public record AssignLeadRequest(Guid AssignedToUserId);