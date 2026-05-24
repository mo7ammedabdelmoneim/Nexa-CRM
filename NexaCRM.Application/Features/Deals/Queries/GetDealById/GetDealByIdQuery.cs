using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Deals.Queries.GetDealById;

public record GetDealByIdQuery(Guid DealId) : IRequest<Result<DealDto>>;