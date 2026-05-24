namespace NexaCRM.Domain.Aggregates.Deals;

public static class DealStage
{
    public const string Proposal = "Proposal";
    public const string Negotiation = "Negotiation";
    public const string Won = "Won";
    public const string Lost = "Lost";

    private static readonly Dictionary<string, string[]> _allowedTransitions = new()
    {
        [Proposal] = [Negotiation, Lost],
        [Negotiation] = [Won, Lost],
        [Won] = [],
        [Lost] = [],
    };

    public static bool CanTransition(string from, string to)
        => _allowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);

    public static IReadOnlyList<string> All =>
        [Proposal, Negotiation, Won, Lost];
}