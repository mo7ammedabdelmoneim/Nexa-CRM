using FluentValidation;
using NexaCRM.Domain.Aggregates.Deals;

namespace NexaCRM.Application.Features.Deals.Commands.UpdateDealStage;

public class UpdateDealStageCommandValidator
    : AbstractValidator<UpdateDealStageCommand>
{
    public UpdateDealStageCommandValidator()
    {
        RuleFor(x => x.DealId)
            .NotEmpty().WithMessage("DealId is required.");

        RuleFor(x => x.NewStage)
            .NotEmpty().WithMessage("Stage is required.")
            .Must(s => DealStage.All.Contains(s))
            .WithMessage($"Stage must be one of: {string.Join(", ", DealStage.All)}");
    }
}