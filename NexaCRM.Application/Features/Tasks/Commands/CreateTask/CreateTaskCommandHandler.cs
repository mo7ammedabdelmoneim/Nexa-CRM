using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;
using NexaCRM.Application.Mappings;
using NexaCRM.Domain.Aggregates.Tasks;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaskDto>> Handle(
        CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = CrmTask.Create(
            command.Title,
            command.Priority,
            command.AssignedToUserId,
            command.TenantId,
            command.CreatedByUserId,
            command.DueDate,
            command.LeadId,
            command.DealId);

        await _taskRepository.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TaskDto>.Success(task.ToDto());
    }
}