namespace Cineaste.Core.Domain;

public readonly record struct RottenTomatoesId
{
    public string Value { get; }

    public RottenTomatoesId(string value) =>
        this.Value = RottenTomatoesIdRegex.IsMatch(Require.NotNull(value))
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value));

    public static RottenTomatoesId? Nullable(string? value) =>
        value is not null ? new(value) : null;
}
