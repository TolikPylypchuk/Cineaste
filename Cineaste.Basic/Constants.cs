namespace Cineaste.Basic;

using System.Text.RegularExpressions;

public static partial class Constants
{
    public static readonly Regex ImdbIdRegex = GenerateImdbIdRegex();
    public static readonly Regex RottenTomatoesIdRegex = GenerateRottenTomatoesIdRegex();

    public const string SeasonTitleNumberPlaceholder = "#";

    public const int MaxNameLength = 100;
    public const int MinYear = 1900;

    [GeneratedRegex("^$|^tt\\d+$", RegexOptions.Compiled)]
    private static partial Regex GenerateImdbIdRegex();

    [GeneratedRegex("^$|^(?:m|tv)/[^\\s]+", RegexOptions.Compiled)]
    private static partial Regex GenerateRottenTomatoesIdRegex();
}
