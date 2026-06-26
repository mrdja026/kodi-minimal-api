using FluentValidation.Results;

namespace KodiMinimalApi.Models;

public static class ErrorHelper
{
    public static ValidationError[] ToErrors(this ValidationResult result)
        => result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToArray();
}
