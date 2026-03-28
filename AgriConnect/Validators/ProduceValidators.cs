using FluentValidation;

public class CreateProduceValidator : AbstractValidator<CreateProduceDto>
{
    public CreateProduceValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Unit).IsInEnum();
        RuleFor(x => x.AvailableUnitsLeft).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Location).NotEmpty();
        RuleFor(x => x.HarvestDate).NotEmpty().LessThanOrEqualTo(_ => DateTime.UtcNow);
        RuleFor(x => x.ExpiryDate).NotEmpty().GreaterThan(x => x.HarvestDate);
    }
}

public class UpdateProduceValidator : AbstractValidator<UpdateProduceDto>
{
    public UpdateProduceValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Unit).IsInEnum();
        RuleFor(x => x.AvailableUnitsLeft).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Location).NotEmpty();
        RuleFor(x => x.HarvestDate).NotEmpty().LessThanOrEqualTo(_ => DateTime.UtcNow);
        RuleFor(x => x.ExpiryDate).NotEmpty().GreaterThan(x => x.HarvestDate);
    }
}