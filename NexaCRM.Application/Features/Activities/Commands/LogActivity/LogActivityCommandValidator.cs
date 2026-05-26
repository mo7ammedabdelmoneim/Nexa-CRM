using FluentValidation;
using NexaCRM.Domain.Aggregates.Activities;

namespace NexaCRM.Application.Features.Activities.Commands.LogActivity;

public class LogActivityCommandValidator : AbstractValidator<LogActivityCommand>
{
    public LogActivityCommandValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required.")
            .Must(t => ActivityType.All.Contains(t))
            .WithMessage($"Type must be one of: {string.Join(", ", ActivityType.All)}");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.OccurredAt)
            .NotEmpty().WithMessage("OccurredAt is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("OccurredAt cannot be in the future.");

        RuleFor(x => x)
            .Must(x => x.LeadId.HasValue || x.DealId.HasValue)
            .WithMessage("Activity must be linked to at least a Lead or a Deal.");
    }
}