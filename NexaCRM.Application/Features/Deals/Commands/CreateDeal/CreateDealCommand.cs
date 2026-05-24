using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Deals.Commands.CreateDeal;

public record CreateDealCommand(
    Guid LeadId,
    string Title,
    decimal Amount,
    Guid TenantId,
    Guid CreatedByUserId,
    string Currency = "USD",
    DateTime? CloseDate = null,
    Guid? AssignedToUserId = null) : IRequest<Result<DealDto>>;