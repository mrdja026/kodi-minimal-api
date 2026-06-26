using FluentValidation;
using KodiMinimalApi.Features;

namespace KodiMinimalApi.Validators;

public class SystemPropertiesValidator : AbstractValidator<SystemPropertiesRequest>
{
    public SystemPropertiesValidator()
    {
        RuleFor(x => x.Properties)
            .NotEmpty()
            .WithMessage("At least one property must be requested.");
    }
}

public class SystemActionValidator : AbstractValidator<SystemActionRequest>
{
    public SystemActionValidator()
    {
        RuleFor(x => x.Confirm)
            .Equal(true)
            .WithMessage("Explicit confirmation (confirm: true) is required for this operation.");
    }
}
