namespace Cineaste.Client.Validation;

using System.Linq.Expressions;

using Cineaste.Client.Localization;

using FluentValidation;
using FluentValidation.Validators;

using Microsoft.Extensions.Localization;

public sealed partial class PropertyValidator<T, TProperty>
{
    public readonly ValidationContext<T> Context = new(default!);

    private readonly IReadOnlyCollection<(IPropertyValidator<T, TProperty> Validator, string ErrorCode)> validators;
    private readonly IStringLocalizer<Resources> localizer;

    public PropertyValidator(
        IValidator<T> validator,
        Expression<Func<T, TProperty?>> property,
        IStringLocalizer<Resources> localizer)
    {
        var propertyName = property.GetMemberName();

        this.validators = validator
            .CreateDescriptor()
            .GetValidatorsForMember(propertyName)
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
        this.validators
            .Select(v => v.Validator.IsValid(this.Context, value) ? null : v.ErrorCode)
            .WhereNotNull()
            .Select(errorCode => this.localizer[$"Validation.{errorCode}"].ToString())
            .ToList()
            .AsReadOnly();
}

public static class PropertyValidator
{
    public static PropertyValidator<T, TProperty> Create<T, TProperty>(
        IValidator<T> validator,
        Expression<Func<T, TProperty?>> property,
        IStringLocalizer<Resources> localizer) =>
        new(validator, property, localizer);
}
