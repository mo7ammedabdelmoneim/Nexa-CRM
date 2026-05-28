using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.AuditLogs.Queries.GetAuditLogs;

public record GetAuditLogsQuery(
    string? EntityType = null,
    Guid? EntityId = null,
    Guid? UserId = null,
    string? Action = null,
    DateTime? From = null,
    DateTime? To = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedResult<AuditLogDto>>>;