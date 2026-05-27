using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.Features.Reports.Queries.GetActivitySummary;
using NexaCRM.Application.Features.Reports.Queries.GetConversionRate;
using NexaCRM.Application.Features.Reports.Queries.GetLeadSources;
using NexaCRM.Application.Features.Reports.Queries.GetPipelineValue;
using NexaCRM.Application.Features.Reports.Queries.GetSalesRepPerformance;

namespace NexaCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public ReportsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

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
            new GetConversionRateQuery(_currentUser.TenantId, fromDate, toDate, assignedToUserId),
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
            new GetPipelineValueQuery(_currentUser.TenantId, assignedToUserId),
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
            new GetSalesRepPerformanceQuery(_currentUser.TenantId),
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
            new GetLeadSourcesQuery(_currentUser.TenantId, fromDate, toDate),
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
            new GetActivitySummaryQuery(_currentUser.TenantId, fromDate, toDate),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }
}