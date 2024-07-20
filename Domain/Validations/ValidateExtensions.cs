using FluentValidation;

namespace Domain.Validations;

public static class ValidateExtensions
{
    public static T ValidateWithErrors<T>(this IValidator<T> validator, T value)
    {
        var validationResult = validator.Validate(value);
        return validationResult.IsValid
            ? value
            : throw new ValidationException(validationResult.Errors);
    }
}
