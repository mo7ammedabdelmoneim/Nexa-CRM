

using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;
using NexaCRM.Application.Mappings;
using NexaCRM.Domain.Aggregates.Activities;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Activities.Commands.LogActivity;

public class LogActivityCommandHandler
    : IRequestHandler<LogActivityCommand, Result<ActivityDto>>
{
    private readonly IActivityRepository _activityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogActivityCommandHandler(
        IActivityRepository activityRepository,
        IUnitOfWork unitOfWork)
    {
        _activityRepository = activityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ActivityDto>> Handle(
        LogActivityCommand command,
        CancellationToken cancellationToken)
    {
        var activity = Activity.Create(
            command.Type,
            command.Description,
            command.OccurredAt,
            command.TenantId,
            command.CreatedByUserId,
            command.LeadId,
            command.DealId);

        await _activityRepository.AddAsync(activity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ActivityDto>.Success(activity.ToDto());
    }
}