namespace Cineaste.Basic;

using System.Text.RegularExpressions;

public static class Constants
{
    public static readonly Regex ImdbIdRegex = new(@"^$|^tt\d+$", RegexOptions.Compiled);
    public static readonly Regex RottenTomatoesIdRegex = new(@"^$|^(?:m|tv)/[^\s]+", RegexOptions.Compiled);
}
