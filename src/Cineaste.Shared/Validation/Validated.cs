namespace Cineaste.Shared.Validation;

public sealed record Validated<T>
    where T : IValidatable<T>
{
    [NotNull]
    public T Value { get; }

    private Validated(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        T.Validator.ValidateAndThrow(value);
        this.Value = value;
    }

    [return: NotNullIfNotNull(nameof(value))]
    public static Validated<T>? Create(T? value) =>
        value is not null ? new Validated<T>(value) : null;
}

public static class ValidationExtensions
{
    extension<T>(T? value)
        where T : IValidatable<T>
    {
        [return: NotNullIfNotNull(nameof(value))]
        public Validated<T>? Validated() =>
            Validation.Validated<T>.Create(value);
    }
}
