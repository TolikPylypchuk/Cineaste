namespace Cineaste.Client.Validation;

using System.Linq.Expressions;

using FluentValidation;
using FluentValidation.Validators;

public sealed partial class PropertyValidator<T, TProperty>
{
    private readonly IReadOnlyCollection<(IPropertyValidator<T, TProperty> Validator, string ErrorCode)> validators;

    public IValidationExecutor Executor { get; }

    public PropertyValidator(
        IValidator<T> validator,
        Expression<Func<T, TProperty?>> property,
        IValidationExecutor executor)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(executor);

        this.Executor = executor;

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
    }

    public IReadOnlyCollection<string> Validate(T instance, TProperty property)
    {
        var ctx = new ValidationContext<T>(instance);

        return this.validators
            .Select(v => v.Validator.IsValid(ctx, property) ? null : v.ErrorCode)
            .WhereNotNull()
            .Select(errorCode => this.Executor.Loc[$"Validation.{errorCode}"].ToString())
            .ToList()
            .AsReadOnly();
    }
}

public static class PropertyValidator
{
    public static PropertyValidator<T, TProperty> Create<T, TProperty>(
        IValidator<T> validator,
        Expression<Func<T, TProperty?>> property,
        IValidationExecutor executor) =>
        new(validator, property, executor);
}
