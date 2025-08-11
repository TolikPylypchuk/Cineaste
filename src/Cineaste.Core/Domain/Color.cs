using System.Text.RegularExpressions;

namespace Cineaste.Core.Domain;

public readonly partial record struct Color
{
    private static readonly Regex ColorHexRegex = GenerateColorHexRegex();

    public string HexValue { get; }

    public Color(string hexValue) =>
        this.HexValue = ColorHexRegex.IsMatch(Require.NotNull(hexValue))
            ? hexValue
            : throw new ArgumentOutOfRangeException(nameof(hexValue));

    public static Color? Nullable(string? value) =>
        value is not null ? new(value) : null;

    [GeneratedRegex("#[0-9A-Fa-f]{6}", RegexOptions.Compiled)]
    private static partial Regex GenerateColorHexRegex();
}
