using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid TaskId) : IRequest<Result>;