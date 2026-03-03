using FluentValidation;
using AgriConnect.DTOs;

namespace AgriConnect.Validators;

public class CreateBlogValidator : AbstractValidator<CreateBlogDto>
{
    private static readonly string[] ValidCategories =
        { "news", "weather", "farmer_problems", "tips", "market" };

    public CreateBlogValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .Must(c => ValidCategories.Contains(c.ToLower()))
            .WithMessage($"Category must be one of: {string.Join(", ", ValidCategories)}.");
    }
}

public class UpdateBlogValidator : AbstractValidator<UpdateBlogDto>
{
    private static readonly string[] ValidCategories =
        { "news", "weather", "farmer_problems", "tips", "market" };

    public UpdateBlogValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .When(x => x.Title != null);

        RuleFor(x => x.Description)
            .MinimumLength(10).WithMessage("Description must be at least 10 characters.")
            .When(x => x.Description != null);

        RuleFor(x => x.Category)
            .Must(c => ValidCategories.Contains(c!.ToLower()))
            .WithMessage($"Category must be one of: {string.Join(", ", ValidCategories)}.")
            .When(x => x.Category != null);
    }
}

public class AddCommentValidator : AbstractValidator<AddCommentDto>
{
    public AddCommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");
    }
}