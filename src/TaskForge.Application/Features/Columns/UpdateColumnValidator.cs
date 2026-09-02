using FluentValidation;

namespace TaskForge.Application.Features.Columns;

public class UpdateColumnValidator : AbstractValidator<UpdateColumnRequest>
{
    public UpdateColumnValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}