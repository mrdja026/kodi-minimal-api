using FluentValidation;
using KodiMinimalApi.Features;

namespace KodiMinimalApi.Validators;

public class FilesSourcesValidator : AbstractValidator<FilesSourcesRequest>
{
    public FilesSourcesValidator()
    {
        RuleFor(x => x.Media)
            .NotEmpty()
            .WithMessage("Media type is required (e.g. video, music, pictures, files, programs).");
    }
}

public class FilesDirectoryValidator : AbstractValidator<FilesDirectoryRequest>
{
    public FilesDirectoryValidator()
    {
        RuleFor(x => x.Directory)
            .NotEmpty()
            .WithMessage("Directory path is required.");
    }
}
