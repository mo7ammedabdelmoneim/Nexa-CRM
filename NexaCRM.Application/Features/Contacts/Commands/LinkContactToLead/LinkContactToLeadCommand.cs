using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Contacts.Commands.LinkContactToLead;

public record LinkContactToLeadCommand(
    Guid ContactId,
    Guid LeadId) : IRequest<Result>;
