using FluentValidation;
using NexaCRM.Domain.Aggregates.Leads;

namespace NexaCRM.Application.Features.Leads.Commands.UpdateLeadStatus;

public class UpdateLeadStatusCommandValidator
    : AbstractValidator<UpdateLeadStatusCommand>
{
    public UpdateLeadStatusCommandValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty().WithMessage("LeadId is required.");

        RuleFor(x => x.NewStatus)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => LeadStatus.All.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", LeadStatus.All)}");
    }
}