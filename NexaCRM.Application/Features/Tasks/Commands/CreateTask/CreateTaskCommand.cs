using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    string Title,
    string Priority,
    Guid AssignedToUserId,
    Guid TenantId,
    Guid CreatedByUserId,
    DateTime? DueDate = null,
    Guid? LeadId = null,
    Guid? DealId = null) : IRequest<Result<TaskDto>>;