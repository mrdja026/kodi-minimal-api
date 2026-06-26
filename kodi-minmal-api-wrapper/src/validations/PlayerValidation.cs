using FluentValidation;
using KodiMinimalApi.Commands;

namespace KodiMinimalApi.Validators;

public class PlayerPlayValidator : AbstractValidator<PlayerPlay>
{
}

public class PlayerPauseValidator : AbstractValidator<PlayerPause>
{
}

public class PlayerStopValidator : AbstractValidator<PlayerStop>
{
}

public class PlayerRequestValidator : AbstractValidator<CommandValue>
{
    public PlayerRequestValidator()
    {
        RuleFor(x => x).SetInheritanceValidator(v =>
        {
            _ = v.Add(new PlayerPlayValidator());
            _ = v.Add(new PlayerPauseValidator());
            _ = v.Add(new PlayerStopValidator());
        });
    }
}
