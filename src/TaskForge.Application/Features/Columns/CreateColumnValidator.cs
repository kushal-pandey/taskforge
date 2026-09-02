using FluentValidation;

namespace TaskForge.Application.Features.Columns;

public class CreateColumnValidator : AbstractValidator<CreateColumnRequest>
{
    public CreateColumnValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}