using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Deals.Commands.UpdateDealDetails;

public record UpdateDealDetailsCommand(
    Guid DealId,
    string Title,
    decimal Amount,
    string Currency,
    DateTime? CloseDate) : IRequest<Result>;