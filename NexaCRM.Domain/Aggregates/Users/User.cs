using NexaCRM.Domain.Common;
using NexaCRM.Domain.Exceptions;

namespace NexaCRM.Domain.Aggregates.Users;

public sealed class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = UserRole.SalesRep;
    public string FullName { get; private set; } = string.Empty;
    public Guid TenantId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    public static User Create(
        string email,
        string passwordHash,
        string fullName,
        Guid tenantId,
        string role = UserRole.SalesRep)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        if (!email.Contains('@'))
            throw new DomainException("Email is not valid.");

        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name is required.");

        if (!UserRole.All.Contains(role))
            throw new DomainException(
                $"Role must be one of: {string.Join(", ", UserRole.All)}");

        return new User
        {
            Email = email.Trim().ToLower(),
            PasswordHash = passwordHash,
            FullName = fullName.Trim(),
            TenantId = tenantId,
            Role = role,
        };
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("User is already inactive.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(string newRole)
    {
        if (!UserRole.All.Contains(newRole))
            throw new DomainException(
                $"Role must be one of: {string.Join(", ", UserRole.All)}");

        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }
}