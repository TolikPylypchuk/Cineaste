namespace Cineaste.Basic;

using System.Text.RegularExpressions;

public static class Constants
{
    public static readonly Regex ImdbIdRegex = new(@"^$|^tt\d+$", RegexOptions.Compiled);
    public static readonly Regex RottenTomatoesLinkRegex = new(
        @"^$|^https://www.rottentomatoes.com/(?:m|tv)/[a-zA-z0-9_-]+/?$", RegexOptions.Compiled);
}
