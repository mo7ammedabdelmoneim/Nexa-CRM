using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Deals.Queries.GetPipelineOverview;

public record GetPipelineOverviewQuery(
    Guid TenantId,
    Guid? AssignedToUserId = null) : IRequest<Result<IReadOnlyList<PipelineStageDto>>>;