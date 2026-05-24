using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Leads.Commands.DeleteLead;

public record DeleteLeadCommand(Guid LeadId) : IRequest<Result>;