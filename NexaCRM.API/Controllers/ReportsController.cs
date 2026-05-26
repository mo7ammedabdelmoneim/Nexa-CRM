using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Features.Reports.Queries.GetActivitySummary;
using NexaCRM.Application.Features.Reports.Queries.GetConversionRate;
using NexaCRM.Application.Features.Reports.Queries.GetLeadSources;
using NexaCRM.Application.Features.Reports.Queries.GetPipelineValue;
using NexaCRM.Application.Features.Reports.Queries.GetSalesRepPerformance;

namespace NexaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;

    private static readonly Guid _tenantId =
        Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    public ReportsController(ISender sender)
        => _sender = sender;

    // GET api/reports/conversion-rate
    [HttpGet("conversion-rate")]
    public async Task<IActionResult> GetConversionRate(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? assignedToUserId,
        CancellationToken cancellationToken = default)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var result = await _sender.Send(
            new GetConversionRateQuery(_tenantId, fromDate, toDate, assignedToUserId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/reports/pipeline-value
    [HttpGet("pipeline-value")]
    public async Task<IActionResult> GetPipelineValue(
        [FromQuery] Guid? assignedToUserId,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetPipelineValueQuery(_tenantId, assignedToUserId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/reports/sales-rep-performance
    [HttpGet("sales-rep-performance")]
    public async Task<IActionResult> GetSalesRepPerformance(
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetSalesRepPerformanceQuery(_tenantId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/reports/lead-sources
    [HttpGet("lead-sources")]
    public async Task<IActionResult> GetLeadSources(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken = default)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var result = await _sender.Send(
            new GetLeadSourcesQuery(_tenantId, fromDate, toDate),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/reports/activity-summary
    [HttpGet("activity-summary")]
    public async Task<IActionResult> GetActivitySummary(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken = default)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var result = await _sender.Send(
            new GetActivitySummaryQuery(_tenantId, fromDate, toDate),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }
}