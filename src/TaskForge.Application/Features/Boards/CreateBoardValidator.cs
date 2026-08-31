using FluentValidation;

namespace TaskForge.Application.Features.Boards;

public class CreateBoardValidator : AbstractValidator<CreateBoardRequest>
{
    public CreateBoardValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}