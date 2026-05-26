using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Domain.Exceptions;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Tasks.Commands.CompleteTask;

public class CompleteTaskCommandHandler
    : IRequestHandler<CompleteTaskCommand, Result>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteTaskCommandHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CompleteTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            command.TaskId, cancellationToken);

        if (task is null)
            return Result.Failure("Task not found.");

        try
        {
            task.Complete();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}