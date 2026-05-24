using NexaCRM.Domain.Common;
using NexaCRM.Domain.Events;
using NexaCRM.Domain.Exceptions;
using NexaCRM.Domain.ValueObjects;

namespace NexaCRM.Domain.Aggregates.Deals;

public sealed class Deal : BaseEntity
{
    public Guid LeadId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Stage { get; private set; } = DealStage.Proposal;
    public MonetaryValue Value { get; private set; } = null!;
    public DateTime? CloseDate { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public Guid TenantId { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private Deal() { }

    // Factory 
    public static Deal Create(
        Guid leadId,
        string title,
        decimal amount,
        Guid tenantId,
        Guid createdByUserId,
        string currency = "USD",
        DateTime? closeDate = null,
        Guid? assignedToUserId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Deal title is required.");

        var deal = new Deal
        {
            LeadId = leadId,
            Title = title.Trim(),
            Value = MonetaryValue.Create(amount, currency),
            TenantId = tenantId,
            CloseDate = closeDate,
            AssignedToUserId = assignedToUserId,
        };

        deal.CreatedByUserId = createdByUserId;

        deal.RaiseDomainEvent(new DealCreatedEvent(
            deal.Id, leadId, title, amount, assignedToUserId));

        return deal;
    }

    // Stage Transition 
    public void TransitionTo(string newStage)
    {
        if (!DealStage.CanTransition(Stage, newStage))
            throw new InvalidStatusTransitionException(Stage, newStage);

        var oldStage = Stage;
        Stage = newStage;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new DealStageChangedEvent(
            Id, Title, oldStage, newStage, AssignedToUserId));

        if (newStage == DealStage.Won)
            RaiseDomainEvent(new DealWonEvent(
                Id, Title, Value.Amount, AssignedToUserId));
    }

    // Update 
    public void UpdateDetails(
        string title,
        decimal amount,
        string currency,
        DateTime? closeDate)
    {
        if (Stage == DealStage.Won || Stage == DealStage.Lost)
            throw new DomainException("Cannot edit a closed deal.");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Deal title is required.");

        Title = title.Trim();
        Value = MonetaryValue.Create(amount, currency);
        CloseDate = closeDate;
        UpdatedAt = DateTime.UtcNow;
    }

    // Assign 
    public void Assign(Guid userId)
    {
        if (Stage == DealStage.Won || Stage == DealStage.Lost)
            throw new DomainException("Cannot reassign a closed deal.");

        AssignedToUserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}