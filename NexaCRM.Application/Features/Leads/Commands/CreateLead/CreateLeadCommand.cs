using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Leads.Commands.CreateLead;

public record CreateLeadCommand(
    string Name,
    string Email,
    string Source,
    Guid TenantId,
    Guid CreatedByUserId,
    string? Phone = null,
    Guid? AssignedToUserId = null) : IRequest<Result<LeadDto>>;