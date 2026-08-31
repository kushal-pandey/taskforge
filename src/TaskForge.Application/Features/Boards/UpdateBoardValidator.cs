using FluentValidation;

namespace TaskForge.Application.Features.Boards;

public class UpdateBoardValidator : AbstractValidator<UpdateBoardRequest>
{
    public UpdateBoardValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}