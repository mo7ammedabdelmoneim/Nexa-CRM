using FluentValidation;
using NexaCRM.Domain.Aggregates.Tasks;

namespace NexaCRM.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required.")
            .Must(p => TaskPriority.All.Contains(p))
            .WithMessage($"Priority must be one of: {string.Join(", ", TaskPriority.All)}");

        RuleFor(x => x.AssignedToUserId)
            .NotEmpty().WithMessage("AssignedToUserId is required.");

        RuleFor(x => x)
            .Must(x => x.LeadId.HasValue || x.DealId.HasValue)
            .WithMessage("Task must be linked to at least a Lead or a Deal.");
    }
}