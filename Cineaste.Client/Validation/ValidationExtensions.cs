namespace Cineaste.Client.Validation;

using System.Linq.Expressions;

using Cineaste.Shared.Validation;

using FluentValidation;
using FluentValidation.Validators;

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.States;

public static class ValidationExtensions
{
    public static ValidationHelper ValidationRule<T, TProperty, TViewModel>(
        this TViewModel vm,
        Expression<Func<TViewModel, TProperty>> property,
        PropertyValidatorExtractor<T> validatorExtractor)
        where T : IValidatable<T>
        where TViewModel : ReactiveValidationObject
    {
        var validators = validatorExtractor.GetValidatorsForProperty(property);

        return vm.ValidationRule(
            property,
            vm.WhenAnyValue(property).Select(value => Validate(value, validatorExtractor.Context, validators)));
    }

    public static PropertyValidatorExtractor<T> UsingValidator<T>(this ReactiveValidationObject _)
        where T : IValidatable<T> =>
        new();

    private static ValidationState Validate<T, TProperty>(
        TProperty value,
        ValidationContext<T> ctx,
        IReadOnlyCollection<(IPropertyValidator<T, TProperty> Validator, string ErrorCode)> validators)
    {
        var results = validators
            .Select(v => v.Validator.IsValid(ctx, value) ? null : $"Validation.{v.ErrorCode}")
            .WhereNotNull()
            .ToArray();

        return new ValidationState(isValid: results.Length == 0, ValidationText.Create(results));
    }
}
