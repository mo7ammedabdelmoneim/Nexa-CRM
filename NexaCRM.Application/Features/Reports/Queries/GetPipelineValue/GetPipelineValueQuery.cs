using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Reports.Queries.GetPipelineValue;

public record GetPipelineValueQuery(
    Guid TenantId,
    Guid? AssignedToUserId = null) : IRequest<Result<PipelineValueReportDto>>;
