using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string Priority,
    DateTime? DueDate) : IRequest<Result>;