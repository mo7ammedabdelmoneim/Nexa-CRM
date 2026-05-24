using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Leads.Queries.GetLeadById;

public record GetLeadByIdQuery(Guid LeadId) : IRequest<Result<LeadDto>>;