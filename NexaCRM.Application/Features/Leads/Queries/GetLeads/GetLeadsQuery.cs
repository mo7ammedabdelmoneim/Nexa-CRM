using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Leads.Queries.GetLeads;

public record GetLeadsQuery(
    Guid TenantId,
    string? Status = null,
    Guid? AssignedToUserId = null,
    string? Source = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedResult<LeadSummaryDto>>>;