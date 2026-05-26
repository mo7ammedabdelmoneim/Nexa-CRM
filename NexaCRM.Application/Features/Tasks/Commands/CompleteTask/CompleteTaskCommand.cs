using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Tasks.Commands.CompleteTask;

public record CompleteTaskCommand(Guid TaskId) : IRequest<Result>;