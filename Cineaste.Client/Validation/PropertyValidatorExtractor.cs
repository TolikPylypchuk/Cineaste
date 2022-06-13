namespace Cineaste.Client.Validation;

using System.Linq.Expressions;

using FluentValidation;
using FluentValidation.Validators;

public sealed class PropertyValidatorExtractor<T>
    where T : IValidatable<T>
{
    public readonly ValidationContext<T> Context = new(default!);

    public IReadOnlyCollection<(IPropertyValidator<T, TProperty> Validator, string ErrorCode)>
        GetValidatorsForProperty<TViewModel, TProperty>(Expression<Func<TViewModel, TProperty>> property) =>
        T.CreateValidator()
            .CreateDescriptor()
            .GetValidatorsForMember(property.GetMemberName())
            .Select(validatorAndOptions =>
                validatorAndOptions.Validator is IPropertyValidator<T, TProperty> validator
                    ? (Validator: validator, validatorAndOptions.Options.ErrorCode)
                    : default)
            .WhereNotDefault()
            .ToList()
            .AsReadOnly();
}
