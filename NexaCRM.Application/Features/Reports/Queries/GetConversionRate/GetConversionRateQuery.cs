using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Reports.Queries.GetConversionRate;

public record GetConversionRateQuery(
    Guid TenantId,
    DateTime From,
    DateTime To,
    Guid? AssignedToUserId = null) : IRequest<Result<ConversionRateDto>>;