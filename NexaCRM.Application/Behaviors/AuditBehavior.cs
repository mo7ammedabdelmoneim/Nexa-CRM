using MediatR;
using NexaCRM.Application.Contracts;
using System.Text.Json;

namespace NexaCRM.Application.Behaviors;

public class AuditBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;

    public AuditBehavior(
        IAuditService auditService,
        ICurrentUserService currentUserService)
    {
        _auditService = auditService;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        var requestName = typeof(TRequest).Name;
        if (!requestName.EndsWith("Command"))
            return response;

        if (!_currentUserService.IsAuthenticated)
            return response;

        try
        {
            var action = ExtractAction(requestName);
            var entityType = ExtractEntityType(requestName);
            var entityId = ExtractEntityId(request);

            if (entityId == Guid.Empty) return response;

            await _auditService.LogAsync(
                entityType,
                entityId,
                action,
                _currentUserService.UserId,
                newValues: JsonSerializer.Serialize(request),
                cancellationToken: cancellationToken);
        }
        catch
        {
            // Audit failures should never break the main flow
        }

        return response;
    }

    private static string ExtractAction(string requestName)
    {
        if (requestName.StartsWith("Create")) return "Created";
        if (requestName.StartsWith("Update")) return "Updated";
        if (requestName.StartsWith("Delete")) return "Deleted";
        if (requestName.StartsWith("Assign")) return "Assigned";
        if (requestName.StartsWith("Convert")) return "Converted";
        if (requestName.Contains("Status")) return "StatusChanged";
        if (requestName.Contains("Stage")) return "StageChanged";
        return "Modified";
    }

    private static string ExtractEntityType(string requestName)
    {
        if (requestName.Contains("Lead")) return "Lead";
        if (requestName.Contains("Deal")) return "Deal";
        if (requestName.Contains("Task")) return "Task";
        if (requestName.Contains("Contact")) return "Contact";
        if (requestName.Contains("Activity")) return "Activity";
        return "Unknown";
    }

    private static Guid ExtractEntityId<T>(T request)
    {
        if (request is null) return Guid.Empty;

        var type = request.GetType();

        var idProperty = type.GetProperties()
            .FirstOrDefault(p =>
                p.Name.EndsWith("Id") &&
                p.PropertyType == typeof(Guid) &&
                p.Name != "TenantId" &&
                p.Name != "CreatedByUserId" &&
                p.Name != "AssignedToUserId");

        if (idProperty is null) return Guid.Empty;

        return (Guid)(idProperty.GetValue(request) ?? Guid.Empty);
    }
}