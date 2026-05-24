namespace NexaCRM.Domain.ValueObjects;
public sealed class MonetaryValue
{
    public decimal Amount { get; }
    public string Currency { get; }

    private MonetaryValue() { Currency = string.Empty; }
    private MonetaryValue(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static MonetaryValue Create(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.", nameof(currency));

        return new MonetaryValue(amount, currency.ToUpper().Trim());
    }

    public override bool Equals(object? obj)
        => obj is MonetaryValue other && Amount == other.Amount && Currency == other.Currency;

    public override int GetHashCode()
        => HashCode.Combine(Amount, Currency);
}