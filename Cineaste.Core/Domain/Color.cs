namespace Cineaste.Core.Domain;

using System.Text.RegularExpressions;

public sealed partial record Color
{
    private static readonly Regex ColorHexRegex = GenerateColorHexRegex();

    public string HexValue { get; }

    public Color(string hexValue) =>
        this.HexValue = ColorHexRegex.IsMatch(Require.NotNull(hexValue))
            ? hexValue
            : throw new ArgumentOutOfRangeException(nameof(hexValue));

    [GeneratedRegex("#[0-9A-Fa-f]{6}", RegexOptions.Compiled)]
    private static partial Regex GenerateColorHexRegex();
}
