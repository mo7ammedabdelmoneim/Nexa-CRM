namespace NexaCRM.Domain.ValueObjects;

public sealed class ContactInfo
{
    public string Email { get; }
    public string? Phone { get; }

    private ContactInfo() { Email = string.Empty; }
    private ContactInfo(string email, string? phone)
    {
        Email = email;
        Phone = phone;
    }

    public static ContactInfo Create(string email, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (!email.Contains('@'))
            throw new ArgumentException("Email is not valid.", nameof(email));

        return new ContactInfo(email.Trim().ToLower(), phone?.Trim());
    }

    public override bool Equals(object? obj)
        => obj is ContactInfo other && Email == other.Email && Phone == other.Phone;

    public override int GetHashCode()
        => HashCode.Combine(Email, Phone);
}