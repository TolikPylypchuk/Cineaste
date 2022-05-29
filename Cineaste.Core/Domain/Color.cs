namespace Cineaste.Core.Domain;

using System.Text.RegularExpressions;

public sealed record Color
{
    private static readonly Regex ColorHexRegex = new("#[0-9A-Fa-f]{6}", RegexOptions.Compiled);

    public string HexValue { get; }

    public Color(string hexValue) =>
        this.HexValue = ColorHexRegex.IsMatch(Require.NotNull(hexValue))
            ? hexValue
            : throw new ArgumentOutOfRangeException(nameof(hexValue));
}
