using NexaCRM.Domain.Common;
using NexaCRM.Domain.Events;
using NexaCRM.Domain.Exceptions;
using NexaCRM.Domain.ValueObjects;

namespace NexaCRM.Domain.Aggregates.Leads;
public sealed class Lead : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Source { get; private set; } = string.Empty;
    public string Status { get; private set; } = LeadStatus.New;
    public ContactInfo ContactInfo { get; private set; } = null!;
    public Guid? AssignedToUserId { get; private set; }
    public Guid TenantId { get; private set; }

    private readonly List<LeadNote> _notes = [];
    public IReadOnlyCollection<LeadNote> Notes => _notes.AsReadOnly();

    private Lead() { } 

    // Factory 
    public static Lead Create(
        string name,
        string email,
        string source,
        Guid tenantId,
        Guid createdByUserId,
        string? phone = null,
        Guid? assignedToUserId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Lead name is required.");

        if (string.IsNullOrWhiteSpace(source))
            throw new DomainException("Lead source is required.");

        var lead = new Lead
        {
            Name = name.Trim(),
            Source = source.Trim(),
            ContactInfo = ContactInfo.Create(email, phone),
            TenantId = tenantId,
        };

        lead.CreatedByUserId = createdByUserId;
        lead.AssignedToUserId = assignedToUserId;

        lead.RaiseDomainEvent(new LeadCreatedEvent(
            lead.Id, lead.Name, email, assignedToUserId));

        return lead;
    }

    //  Status Transition 
    public void TransitionTo(string newStatus)
    {
        if (!LeadStatus.CanTransition(Status, newStatus))
            throw new InvalidStatusTransitionException(Status, newStatus);

        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new LeadStatusChangedEvent(
            Id, Name, oldStatus, newStatus, AssignedToUserId));
    }

    // Assign 
    public void Assign(Guid userId)
    {
        if (Status == LeadStatus.Won)
            throw new DomainException("Cannot reassign a won lead.");

        AssignedToUserId = userId;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new LeadAssignedEvent(Id, Name, userId));
    }

    // Update Details 
    public void UpdateDetails(string name, string email, string source, string? phone = null)
    {
        if (Status == LeadStatus.Won)
            throw new DomainException("Cannot edit a won lead.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Lead name is required.");

        Name = name.Trim();
        Source = source.Trim();
        ContactInfo = ContactInfo.Create(email, phone);
        UpdatedAt = DateTime.UtcNow;
    }

    // Add Note 
    public void AddNote(string content, Guid createdByUserId)
    {
        var note = LeadNote.Create(Id, content, createdByUserId);
        _notes.Add(note);
        UpdatedAt = DateTime.UtcNow;
    }

    // Convert 
    public void MarkAsConverted(Guid dealId)
    {
        if (Status != LeadStatus.Qualified && Status != LeadStatus.Negotiation)
            throw new DomainException(
                "Lead must be in Qualified or Negotiation stage to be converted.");

        Status = LeadStatus.Won;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new LeadConvertedEvent(Id, dealId, Name));
    }

    // Delete 
    public new void SoftDelete()
    {
        if (Status == LeadStatus.Won)
            throw new DomainException("Cannot delete a won lead.");

        base.SoftDelete();
    }
}