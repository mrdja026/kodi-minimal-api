using FluentValidation;
using KodiMinimalApi.Commands;

namespace KodiMinimalApi.Validators;

public class VolumeGetValidator : AbstractValidator<VolumeGet>
{
}

public class VolumeSetValidator : AbstractValidator<VolumeSet>
{
    public VolumeSetValidator()
    {
        RuleFor(x => x.Level).InclusiveBetween(0, 100).WithMessage("Volume must be between 0 and 100.");
    }
}

public class VolumeUpValidator : AbstractValidator<VolumeUp>
{
    public VolumeUpValidator()
    {
        RuleFor(x => x.Level).InclusiveBetween(0, 100).WithMessage("Volume can be set from 0 - 100");
    }
}

public class VolumeDownValidator : AbstractValidator<VolumeDown>
{
    public VolumeDownValidator()
    {
        RuleFor(x => x.Level)
            .InclusiveBetween(1, 100)
            .WithMessage("Volume decrease level must be between 1 and 100.");
    }
}

public class VolumeRequestValidator : AbstractValidator<CommandValue>
{
    public VolumeRequestValidator()
    {
        RuleFor(x => x).SetInheritanceValidator(v =>
        {
            _ = v.Add(new VolumeGetValidator());
            _ = v.Add(new VolumeSetValidator());
            _ = v.Add(new VolumeUpValidator());
            _ = v.Add(new VolumeDownValidator());
        });
    }
}