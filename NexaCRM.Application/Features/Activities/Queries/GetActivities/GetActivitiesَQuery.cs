using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Activities.Queries.GetActivities;

public record GetActivitiesQuery(
    Guid TenantId,
    Guid? LeadId = null,
    Guid? DealId = null,
    string? Type = null,
    DateTime? From = null,
    DateTime? To = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedResult<ActivityDto>>>;