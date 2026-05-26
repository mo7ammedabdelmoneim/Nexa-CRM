using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Reports.Queries.GetLeadSources;

public record GetLeadSourcesQuery(
    Guid TenantId,
    DateTime From,
    DateTime To) : IRequest<Result<IReadOnlyList<LeadSourceBreakdownDto>>>;