namespace Cineaste.Shared.Validation;

using System.Linq.Expressions;

public abstract class CineasteValidator<T> : AbstractValidator<T>
    where T : IValidatable<T>
{
    protected static readonly string Contains = nameof(Contains);
    protected static readonly string Distinct = nameof(Distinct);
    protected static readonly string Empty = nameof(Empty);
    protected static readonly string Invalid = nameof(Invalid);
    protected static readonly string Null = nameof(Null);
    protected static readonly string TooLong = nameof(TooLong);
    protected static readonly string TooLow = nameof(TooLow);
    protected static readonly string MustBeTrue = nameof(MustBeTrue);
    protected static readonly string Overlap = nameof(Overlap);
    protected static readonly string Sequence = nameof(Sequence);

    private readonly string errorCodePrefix;

    protected CineasteValidator(string errorCodePrefix) =>
        this.errorCodePrefix = errorCodePrefix;

    protected string ErrorCode<TProp>(Expression<Func<T, TProp>> property, params string[] errorComponents) =>
        $"{this.errorCodePrefix}.{property.GetMemberName()}.{String.Join(".", errorComponents)}";

    protected string ErrorCode(params string[] errorComponents) =>
        $"{this.errorCodePrefix}.{String.Join(".", errorComponents)}";
}
