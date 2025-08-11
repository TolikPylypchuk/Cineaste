namespace Cineaste.Core.Domain;

public readonly record struct ImdbId
{
    public string Value { get; }

    public ImdbId(string value) =>
        this.Value = ImdbIdRegex.IsMatch(Require.NotNull(value))
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value));

    public static ImdbId? Nullable(string? value) =>
        value is not null ? new(value) : null;
}
