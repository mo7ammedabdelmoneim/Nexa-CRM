namespace NexaCRM.Domain.Aggregates.Leads;

public static class LeadStatus
{
    public const string New = "New";
    public const string Contacted = "Contacted";
    public const string Qualified = "Qualified";
    public const string Negotiation = "Negotiation";
    public const string Won = "Won";
    public const string Lost = "Lost";

    private static readonly Dictionary<string, string[]> _allowedTransitions = new()
    {
        [New] = [Contacted],
        [Contacted] = [Qualified, Lost],
        [Qualified] = [Negotiation, Lost],
        [Negotiation] = [Won, Lost],
        [Won] = [],
        [Lost] = [New],
    };

    public static bool CanTransition(string from, string to)
        => _allowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);

    public static IReadOnlyList<string> All =>
        [New, Contacted, Qualified, Negotiation, Won, Lost];
}