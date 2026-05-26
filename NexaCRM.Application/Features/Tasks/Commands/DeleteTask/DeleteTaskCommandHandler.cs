using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler
    : IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskCommandHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            command.TaskId, cancellationToken);

        if (task is null)
            return Result.Failure("Task not found.");

        task.SoftDelete();
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}