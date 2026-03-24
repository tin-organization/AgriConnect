using FluentValidation;

public class CreateEquipmentValidator : AbstractValidator<CreateEquipmentDto>
{
    public CreateEquipmentValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Location).NotEmpty();
        RuleFor(x => x.ManufacturingDate).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.ListingType).IsInEnum();
        RuleFor(x => x.Condition).IsInEnum();
    }
}

public class UpdateEquipmentValidator : AbstractValidator<UpdateEquipmentDto>
{
    public UpdateEquipmentValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Location).NotEmpty();
        RuleFor(x => x.ManufacturingDate).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.ListingType).IsInEnum();
        RuleFor(x => x.Condition).IsInEnum();
    }
}