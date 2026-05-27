using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NexaCRM.Application.Contracts;

namespace NexaCRM.Infrastructure.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User
        => _httpContextAccessor.HttpContext?.User;

    public Guid UserId
        => Guid.Parse(User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User?.FindFirstValue("sub")
            ?? Guid.Empty.ToString());

    public Guid TenantId
        => Guid.Parse(User?.FindFirstValue("tenantId")
            ?? Guid.Empty.ToString());

    public string Role
        => User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    public bool IsAuthenticated
        => User?.Identity?.IsAuthenticated ?? false;
}