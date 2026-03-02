using FluentValidation;

public class AskQuestionValidator : AbstractValidator<AskQuestionDto>
{
    public AskQuestionValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Question cannot be empty.")
            .MinimumLength(5).WithMessage("Question too short.")
            .MaximumLength(1000).WithMessage("Question too long.");
    }
}