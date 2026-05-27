namespace NexaCRM.Domain.Aggregates.Users;

public sealed class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string tokenHash, int expiryDays = 7)
        => new()
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
        };

    public bool IsValid() => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    public void Revoke()
    {
        IsRevoked = true;
    }
}