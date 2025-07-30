using System.Text.RegularExpressions;

namespace Cineaste.Basic;

public static partial class Constants
{
    public static readonly Regex ImdbIdRegex = GenerateImdbIdRegex();
    public static readonly Regex RottenTomatoesIdRegex = GenerateRottenTomatoesIdRegex();

    public const string SeasonTitleNumberPlaceholder = "#";

    public const int FirstSequenceNumber = 1;

    public const int MaxNameLength = 100;
    public const int MinYear = 1900;

    [GeneratedRegex("^$|^tt\\d+$", RegexOptions.Compiled)]
    private static partial Regex GenerateImdbIdRegex();

    [GeneratedRegex("^$|^(?:m|tv)/[^\\s]+", RegexOptions.Compiled)]
    private static partial Regex GenerateRottenTomatoesIdRegex();
}
