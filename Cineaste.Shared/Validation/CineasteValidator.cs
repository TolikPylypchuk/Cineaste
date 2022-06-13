namespace Cineaste.Shared.Validation;

using System.Linq.Expressions;

public abstract class CineasteValidator<T> : AbstractValidator<T>
    where T : IValidatable<T>
{
    private readonly string errorCodePrefix;

    public CineasteValidator(string errorCodePrefix) =>
        this.errorCodePrefix = errorCodePrefix;

    protected string ErrorCode(Expression<Func<T, object>> property, params string[] errorComponents) =>
        $"{this.errorCodePrefix}.{property.GetMemberName()}.{String.Join(".", errorComponents)}";
}
