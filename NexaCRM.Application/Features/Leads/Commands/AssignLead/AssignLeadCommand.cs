using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Leads.Commands.AssignLead;

public record AssignLeadCommand(
    Guid LeadId,
    Guid AssignedToUserId) : IRequest<Result>;