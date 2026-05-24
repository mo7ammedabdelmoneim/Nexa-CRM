using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Deals.Commands.AssignDeal;

public record AssignDealCommand(
    Guid DealId,
    Guid AssignedToUserId) : IRequest<Result>;