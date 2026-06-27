using FluentValidation;
using KodiMinimalApi.Commands;

namespace KodiMinimalApi.Validators;

public class TVShowListValidator : AbstractValidator<TVShowList>
{
    public TVShowListValidator()
    {
        When(x => x.Start.HasValue, () => RuleFor(x => x.Start!.Value).GreaterThanOrEqualTo(0));
        When(x => x.End.HasValue, () => RuleFor(x => x.End!.Value).GreaterThan(0));
    }
}

public class TVShowSearchValidator : AbstractValidator<TVShowSearch>
{
    public TVShowSearchValidator()
    {
        RuleFor(x => x.Query).NotEmpty();
    }
}

public class TVShowSeasonsValidator : AbstractValidator<TVShowSeasons>
{
    public TVShowSeasonsValidator()
    {
        RuleFor(x => x.TVShowId).GreaterThan(0);
    }
}

public class TVShowEpisodesValidator : AbstractValidator<TVShowEpisodes>
{
    public TVShowEpisodesValidator()
    {
        RuleFor(x => x.TVShowId).GreaterThan(0);
        When(x => x.Season.HasValue, () => RuleFor(x => x.Season!.Value).GreaterThanOrEqualTo(0));
    }
}

public class TVShowRecentValidator : AbstractValidator<TVShowRecent>
{
    public TVShowRecentValidator()
    {
        When(x => x.Limit.HasValue, () => RuleFor(x => x.Limit!.Value).GreaterThan(0));
    }
}

public class TVShowPlayEpisodeValidator : AbstractValidator<TVShowPlayEpisode>
{
    public TVShowPlayEpisodeValidator()
    {
        RuleFor(x => x.EpisodeId).GreaterThan(0);
    }
}

public class TVShowRequestValidator : AbstractValidator<CommandValue>
{
    public TVShowRequestValidator()
    {
        RuleFor(x => x).SetInheritanceValidator(v =>
        {
            _ = v.Add(new TVShowListValidator());
            _ = v.Add(new TVShowSearchValidator());
            _ = v.Add(new TVShowSeasonsValidator());
            _ = v.Add(new TVShowEpisodesValidator());
            _ = v.Add(new TVShowRecentValidator());
            _ = v.Add(new TVShowPlayEpisodeValidator());
        });
    }
}
