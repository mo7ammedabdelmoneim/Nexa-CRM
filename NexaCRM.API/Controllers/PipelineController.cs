using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Features.Deals.Commands.AssignDeal;
using NexaCRM.Application.Features.Deals.Commands.CreateDeal;
using NexaCRM.Application.Features.Deals.Commands.UpdateDealDetails;
using NexaCRM.Application.Features.Deals.Commands.UpdateDealStage;
using NexaCRM.Application.Features.Deals.Queries.GetDealById;
using NexaCRM.Application.Features.Deals.Queries.GetDeals;
using NexaCRM.Application.Features.Deals.Queries.GetPipelineOverview;

namespace NexaCRM.API.Controllers;

[ApiController]
[Route("api/pipeline")]
public class PipelineController : ControllerBase
{
    private readonly ISender _sender;

    private static readonly Guid _tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    private static readonly Guid _userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7");

    public PipelineController(ISender sender)
        => _sender = sender;

    // GET api/pipeline
    [HttpGet]
    public async Task<IActionResult> GetOverview(
        [FromQuery] Guid? assignedToUserId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetPipelineOverviewQuery(_tenantId, assignedToUserId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/pipeline/deals
    [HttpGet("deals")]
    public async Task<IActionResult> GetDeals(
        [FromQuery] string? stage,
        [FromQuery] Guid? assignedToUserId,
        [FromQuery] decimal? minValue,
        [FromQuery] decimal? maxValue,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetDealsQuery(_tenantId, stage, assignedToUserId, minValue, maxValue, page, pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/pipeline/deals/{id}
    [HttpGet("deals/{id:guid}")]
    public async Task<IActionResult> GetDealById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetDealByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    // POST api/pipeline/deals
    [HttpPost("deals")]
    public async Task<IActionResult> CreateDeal(
        [FromBody] CreateDealRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateDealCommand(
            request.LeadId,
            request.Title,
            request.Amount,
            _tenantId,
            _userId,
            request.Currency,
            request.CloseDate,
            request.AssignedToUserId), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetDealById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    // PATCH api/pipeline/deals/{id}/stage
    [HttpPatch("deals/{id:guid}/stage")]
    public async Task<IActionResult> UpdateStage(
        Guid id,
        [FromBody] UpdateDealStageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateDealStageCommand(id, request.NewStage), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    // PUT api/pipeline/deals/{id}
    [HttpPut("deals/{id:guid}")]
    public async Task<IActionResult> UpdateDetails(
        Guid id,
        [FromBody] UpdateDealDetailsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateDealDetailsCommand(id, request.Title, request.Amount, request.Currency, request.CloseDate),
            cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    // PATCH api/pipeline/deals/{id}/assign
    [HttpPatch("deals/{id:guid}/assign")]
    public async Task<IActionResult> Assign(
        Guid id,
        [FromBody] AssignDealRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new AssignDealCommand(id, request.AssignedToUserId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }
}

// Request Models 
public record CreateDealRequest(
    Guid LeadId,
    string Title,
    decimal Amount,
    string Currency = "USD",
    DateTime? CloseDate = null,
    Guid? AssignedToUserId = null);

public record UpdateDealStageRequest(string NewStage);
public record UpdateDealDetailsRequest(string Title, decimal Amount, string Currency, DateTime? CloseDate);
public record AssignDealRequest(Guid AssignedToUserId);