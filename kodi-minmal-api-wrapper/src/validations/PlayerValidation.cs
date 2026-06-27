using FluentValidation;
using KodiMinimalApi.Commands;

namespace KodiMinimalApi.Validators;

public class PlayerPlayValidator : AbstractValidator<PlayerPlay>
{
    public PlayerPlayValidator()
    {
        When(x => x.File is not null, () => RuleFor(x => x.File).NotEmpty());
    }
}

public class PlayerPauseValidator : AbstractValidator<PlayerPause>
{
}

public class PlayerStopValidator : AbstractValidator<PlayerStop>
{
}

public class PlayerSeekForwardValidator : AbstractValidator<PlayerSeekForward>
{
    public PlayerSeekForwardValidator()
    {
        RuleFor(x => x.Seconds).InclusiveBetween(1, 600);
    }
}

public class PlayerSeekBackwardValidator : AbstractValidator<PlayerSeekBackward>
{
    public PlayerSeekBackwardValidator()
    {
        RuleFor(x => x.Seconds).InclusiveBetween(1, 600);
    }
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
            _ = v.Add(new PlayerSeekForwardValidator());
            _ = v.Add(new PlayerSeekBackwardValidator());
        });
    }
}
