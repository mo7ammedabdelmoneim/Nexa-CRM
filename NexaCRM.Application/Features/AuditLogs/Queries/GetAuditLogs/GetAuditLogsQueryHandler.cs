using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery, Result<PaginatedResult<AuditLogDto>>>
{
    private readonly IAuditLogQueryRepository _auditLogQueryRepository;

    public GetAuditLogsQueryHandler(
        IAuditLogQueryRepository auditLogQueryRepository)
        => _auditLogQueryRepository = auditLogQueryRepository;

    public async Task<Result<PaginatedResult<AuditLogDto>>> Handle(
        GetAuditLogsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _auditLogQueryRepository.GetPagedAsync(
            query.EntityType,
            query.EntityId,
            query.UserId,
            query.Action,
            query.From,
            query.To,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result<PaginatedResult<AuditLogDto>>.Success(result);
    }
}