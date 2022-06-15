namespace Cineaste.Client.Validation;

using Cineaste.Client.Localization;

using FluentValidation;
using FluentValidation.Validators;

using Microsoft.Extensions.Localization;

public sealed partial class PropertyValidator<T, TProperty>
{
    public readonly ValidationContext<T> Context = new(default!);

    private readonly IReadOnlyCollection<(IPropertyValidator<T, TProperty> Validator, string ErrorCode)> validators;
    private readonly IStringLocalizer<Resources> localizer;

    public PropertyValidator(IValidator<T> validator, string property, IStringLocalizer<Resources> localizer)
    {
        this.validators = validator
            .CreateDescriptor()
            .GetValidatorsForMember(property)
            .Select(validatorAndOptions =>
                validatorAndOptions.Validator is IPropertyValidator<T, TProperty> validator
                    ? (Validator: validator, validatorAndOptions.Options.ErrorCode)
                    : default)
            .WhereNotDefault()
            .ToList()
            .AsReadOnly();

        this.localizer = localizer;
    }

    public IReadOnlyCollection<string> Validate(TProperty value) =>
        validators
            .Select(v => v.Validator.IsValid(this.Context, value) ? null : v.ErrorCode)
            .WhereNotNull()
            .Select(errorCode => this.localizer[$"Validation.{errorCode}"].ToString())
            .ToList()
            .AsReadOnly();
}
