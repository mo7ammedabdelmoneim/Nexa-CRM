using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Leads.Commands.UpdateLeadStatus;

public record UpdateLeadStatusCommand(
    Guid LeadId,
    string NewStatus) : IRequest<Result>;