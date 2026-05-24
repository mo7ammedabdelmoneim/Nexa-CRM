namespace NexaCRM.Domain.Aggregates.Leads;
public sealed class LeadNote
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LeadId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private LeadNote() { } 

    internal static LeadNote Create(Guid leadId, string content, Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Note content cannot be empty.", nameof(content));

        return new LeadNote
        {
            LeadId = leadId,
            Content = content.Trim(),
            CreatedByUserId = createdByUserId
        };
    }
}