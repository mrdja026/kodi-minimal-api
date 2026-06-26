using FluentValidation;
using KodiMinimalApi.Features;

namespace KodiMinimalApi.Validators;

public class FilesDirectoryValidator : AbstractValidator<FilesDirectoryRequest>
{
    public FilesDirectoryValidator()
    {
        RuleFor(x => x.Directory)
            .NotEmpty()
            .WithMessage("Directory path is required.");
    }
}
