namespace Cineaste.Shared.Validation;

public sealed record Validated<T>
    where T : IValidatable<T>
{
    [NotNull]
    public T Value { get; }

    private Validated(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        T.CreateValidator().ValidateAndThrow(value);
        this.Value = value;
    }

    [return: NotNullIfNotNull("value")]
    public static Validated<T>? Create(T? value) =>
        value is not null ? new Validated<T>(value) : null;
}

public static class ValidationExtensions
{
    [return: NotNullIfNotNull("value")]
    public static Validated<T>? Validated<T>(this T? value)
        where T : IValidatable<T> =>
        Validation.Validated<T>.Create(value);
}
