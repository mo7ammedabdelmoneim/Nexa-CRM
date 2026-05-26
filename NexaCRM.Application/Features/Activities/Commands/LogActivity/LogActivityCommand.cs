using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Activities.Commands.LogActivity;

public record LogActivityCommand(
    string Type,
    string Description,
    DateTime OccurredAt,
    Guid TenantId,
    Guid CreatedByUserId,
    Guid? LeadId = null,
    Guid? DealId = null) : IRequest<Result<ActivityDto>>;