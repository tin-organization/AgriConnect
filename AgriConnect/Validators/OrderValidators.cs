using FluentValidation;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.ItemType)
            .IsInEnum().WithMessage("Invalid item type. Must be Produce or Equipment.");

        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("A valid item ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .When(x => x.ItemType == ItemType.Produce);
    }
}

public class UpdateOrderValidator : AbstractValidator<UpdateOrderDto>
{
    public UpdateOrderValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}