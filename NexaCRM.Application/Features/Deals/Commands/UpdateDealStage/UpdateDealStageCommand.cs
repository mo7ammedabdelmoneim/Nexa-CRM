using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Deals.Commands.UpdateDealStage;

public record UpdateDealStageCommand(
    Guid DealId,
    string NewStage) : IRequest<Result>;