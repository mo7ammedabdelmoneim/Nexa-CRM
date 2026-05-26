using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Features.Tasks.Commands.CompleteTask;
using NexaCRM.Application.Features.Tasks.Commands.CreateTask;
using NexaCRM.Application.Features.Tasks.Commands.DeleteTask;
using NexaCRM.Application.Features.Tasks.Commands.UpdateTask;
using NexaCRM.Application.Features.Tasks.Queries.GetOverdueTasks;
using NexaCRM.Application.Features.Tasks.Queries.GetTasks;

namespace NexaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ISender _sender;

    private static readonly Guid _tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    private static readonly Guid _userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7");

    public TasksController(ISender sender)
        => _sender = sender;

    // POST api/tasks
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateTaskCommand(
            request.Title,
            request.Priority,
            request.AssignedToUserId,
            _tenantId,
            _userId,
            request.DueDate,
            request.LeadId,
            request.DealId), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/tasks
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? assignedToUserId,
        [FromQuery] bool? isCompleted,
        [FromQuery] string? priority,
        [FromQuery] DateTime? dueBefore,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetTasksQuery(
            _tenantId, assignedToUserId, isCompleted, priority, dueBefore, page, pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/tasks/overdue
    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetOverdueTasksQuery(_tenantId), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // PUT api/tasks/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateTaskCommand(
            id, request.Title, request.Priority, request.DueDate),
            cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    // PATCH api/tasks/{id}/complete
    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> Complete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CompleteTaskCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    // DELETE api/tasks/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new DeleteTaskCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }
}

public record CreateTaskRequest(
    string Title,
    string Priority,
    Guid AssignedToUserId,
    DateTime? DueDate,
    Guid? LeadId,
    Guid? DealId);

public record UpdateTaskRequest(
    string Title,
    string Priority,
    DateTime? DueDate);