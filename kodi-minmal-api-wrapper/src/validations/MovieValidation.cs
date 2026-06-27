using FluentValidation;
using KodiMinimalApi.Commands;

namespace KodiMinimalApi.Validators;

public class MovieListValidator : AbstractValidator<MovieList>
{
    public MovieListValidator()
    {
        When(x => x.Start.HasValue, () => RuleFor(x => x.Start!.Value).GreaterThanOrEqualTo(0));
        When(x => x.End.HasValue, () => RuleFor(x => x.End!.Value).GreaterThan(0));
    }
}

public class MovieSearchValidator : AbstractValidator<MovieSearch>
{
    public MovieSearchValidator()
    {
        RuleFor(x => x.Query).NotEmpty();
    }
}

public class MovieRecentValidator : AbstractValidator<MovieRecent>
{
    public MovieRecentValidator()
    {
        When(x => x.Limit.HasValue, () => RuleFor(x => x.Limit!.Value).GreaterThan(0));
    }
}

public class MoviePlayValidator : AbstractValidator<MoviePlay>
{
    public MoviePlayValidator()
    {
        RuleFor(x => x.MovieId).GreaterThan(0);
    }
}

public class MovieScanValidator : AbstractValidator<MovieScan>
{
    public MovieScanValidator()
    {
        When(x => x.Directory is not null, () => RuleFor(x => x.Directory).NotEmpty());
    }
}

public class MovieScanMoviesValidator : AbstractValidator<MovieScanMovies>
{
}

public class MovieScanTVValidator : AbstractValidator<MovieScanTV>
{
}

public class MovieSearchDirValidator : AbstractValidator<MovieSearchDir>
{
    public MovieSearchDirValidator()
    {
        When(x => x.Query is not null, () => RuleFor(x => x.Query).NotEmpty());
        When(x => x.Directory is not null, () => RuleFor(x => x.Directory).NotEmpty());
    }
}

public class MovieRequestValidator : AbstractValidator<CommandValue>
{
    public MovieRequestValidator()
    {
        RuleFor(x => x).SetInheritanceValidator(v =>
        {
            _ = v.Add(new MovieListValidator());
            _ = v.Add(new MovieSearchValidator());
            _ = v.Add(new MovieRecentValidator());
            _ = v.Add(new MoviePlayValidator());
            _ = v.Add(new MovieScanValidator());
            _ = v.Add(new MovieScanMoviesValidator());
            _ = v.Add(new MovieScanTVValidator());
            _ = v.Add(new MovieSearchDirValidator());
        });
    }
}
