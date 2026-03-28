using FluentValidation;

public class ToggleProduceAvailabilityValidator : AbstractValidator<ToggleProduceAvailabilityDto>
{
    public ToggleProduceAvailabilityValidator()
    {
        RuleFor(x => x.MakeAvailable).NotNull();
    }
}