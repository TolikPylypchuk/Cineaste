using System.Linq.Expressions;

namespace Cineaste.Shared.Validation;

public abstract class CineasteValidator<T>(string errorCodePrefix) : AbstractValidator<T>
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

    protected static readonly string IndexPlaceholder = "{CollectionIndex}";

    protected string ErrorCode<TProp>(Expression<Func<T, TProp>> property, params string?[] errorComponents) =>
        $"{errorCodePrefix}.{property.GetMemberName()}." +
        $"{String.Join(".", errorComponents.Where(c => !String.IsNullOrWhiteSpace(c)))}";

    protected string ErrorCode(params string?[] errorComponents) =>
        $"{errorCodePrefix}.{String.Join(".", errorComponents.Where(c => !String.IsNullOrWhiteSpace(c)))}";
}
