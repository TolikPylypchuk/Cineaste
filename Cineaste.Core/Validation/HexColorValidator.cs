namespace Cineaste.Core.Validation;

using System.Text.RegularExpressions;

public static class HexColorValidator
{
    private static readonly Regex HexString = new(@"\A\b[0-9a-fA-F]+\b\Z", RegexOptions.Compiled);

    public static bool IsArgbString(this string? color) =>
        color != null && color.Length == 9 && color.StartsWith('#') && HexString.IsMatch(color[1..]);
}
