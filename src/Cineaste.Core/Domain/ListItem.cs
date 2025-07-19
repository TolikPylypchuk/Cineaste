using System.Text.RegularExpressions;

using static Cineaste.Core.Domain.Constants;

namespace Cineaste.Core.Domain;

file static class Constants
{
    public const string Space = " ";
    public const string NumberDelimiter = ":";
}

public sealed partial class ListItem : Entity<ListItem>
{
    private static readonly Regex NumberRegex = CreateNumberRegex();
    private static readonly Regex SpacesOrOtherCharactersRegex = CreateSpacesOrOtherCharactersRegex();

    public CineasteList List { get; private set; }

    public Movie? Movie { get; private set; }
    public Series? Series { get; private set; }
    public Franchise? Franchise { get; private set; }

    public string NormalizedTitle { get; private set; }
    public string NormalizedOriginalTitle { get; private set; }

    public string NormalizedShortTitle { get; private set; }
    public string NormalizedShortOriginalTitle { get; private set; }

    public int StartYear { get; private set; }
    public int EndYear { get; private set; }

    public int SequenceNumber { get; set; }

    public Color? ActiveColor { get; set; }

    public bool IsShown { get;private set; }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private ListItem(Id<ListItem> id)
        : base(id)
    {
        this.List = null!;
        this.NormalizedTitle = String.Empty;
        this.NormalizedOriginalTitle = String.Empty;
        this.NormalizedShortTitle = String.Empty;
        this.NormalizedShortOriginalTitle = String.Empty;
        this.ActiveColor = null;
    }

    public ListItem(Id<ListItem> id, CineasteList list, Movie movie)
        : base(id)
    {
        this.List = list;
        this.Movie = movie;

        this.SetProperties(movie);
        movie.ListItem = this;
    }

    public ListItem(Id<ListItem> id, CineasteList list, Series series)
        : base(id)
    {
        this.List = list;
        this.Series = series;

        this.SetProperties(series);
        series.ListItem = this;
    }

    public ListItem(Id<ListItem> id, CineasteList list, Franchise franchise)
        : base(id)
    {
        this.List = list;
        this.Franchise = franchise;

        this.SetProperties(franchise);
        franchise.ListItem = this;
    }

    [MemberNotNull(
        nameof(this.NormalizedShortTitle),
        nameof(this.NormalizedShortOriginalTitle),
        nameof(this.NormalizedTitle),
        nameof(this.NormalizedOriginalTitle))]
    public void SetProperties(Movie movie)
    {
        this.NormalizedShortTitle = this.Normalize(movie.Title.Name);
        this.NormalizedShortOriginalTitle = this.Normalize(movie.OriginalTitle.Name);

        this.NormalizedTitle = this.CreateFullTitle(movie.FranchiseItem, this.NormalizedShortTitle);
        this.NormalizedOriginalTitle = this.CreateFullTitle(movie.FranchiseItem, this.NormalizedShortOriginalTitle);

        this.StartYear = movie.Year;
        this.EndYear = movie.Year;

        this.IsShown = true;
        this.ActiveColor = movie.GetActiveColor();
    }

    [MemberNotNull(
        nameof(this.NormalizedShortTitle),
        nameof(this.NormalizedShortOriginalTitle),
        nameof(this.NormalizedTitle),
        nameof(this.NormalizedOriginalTitle))]
    public void SetProperties(Series series)
    {
        this.NormalizedShortTitle = this.Normalize(series.Title.Name);
        this.NormalizedShortOriginalTitle = this.Normalize(series.OriginalTitle.Name);

        this.NormalizedTitle = this.CreateFullTitle(series.FranchiseItem, this.NormalizedShortTitle);
        this.NormalizedOriginalTitle = this.CreateFullTitle(series.FranchiseItem, this.NormalizedShortOriginalTitle);

        this.StartYear = series.StartYear;
        this.EndYear = series.EndYear;

        this.IsShown = true;
        this.ActiveColor = series.GetActiveColor();
    }

    [MemberNotNull(
        nameof(this.NormalizedShortTitle),
        nameof(this.NormalizedShortOriginalTitle),
        nameof(this.NormalizedTitle),
        nameof(this.NormalizedOriginalTitle))]
    public void SetProperties(Franchise franchise)
    {
        this.NormalizedShortTitle = this.Normalize(franchise.ActualTitle?.Name);
        this.NormalizedShortOriginalTitle = this.Normalize(franchise.ActualOriginalTitle?.Name);

        this.NormalizedTitle = this.CreateFullTitle(franchise.FranchiseItem, this.NormalizedShortTitle);
        this.NormalizedOriginalTitle = this.CreateFullTitle(franchise.FranchiseItem, this.NormalizedShortOriginalTitle);

        this.StartYear = franchise.StartYear ?? 0;
        this.EndYear = franchise.EndYear ?? 0;

        this.IsShown = franchise.ShowTitles;
        this.ActiveColor = franchise.GetActiveColor();
    }

    [GeneratedRegex("([0-9]+)", RegexOptions.Compiled)]
    private static partial Regex CreateNumberRegex();

    [GeneratedRegex("\\s+|\\p{C}+", RegexOptions.Compiled)]
    private static partial Regex CreateSpacesOrOtherCharactersRegex();

    private string CreateFullTitle(FranchiseItem? item, string title) =>
        item is null
            ? title
            : this.CreateFullTitle(
                item.ParentFranchise.FranchiseItem,
                $"{this.Normalize(item.ParentFranchise.ActualTitle?.Name)}/" +
                $"{this.DelimitNumber(item.SequenceNumber)}/" +
                $"{title}");

    private string Normalize(string? str)
    {
        if (String.IsNullOrEmpty(str))
        {
            return String.Empty;
        }

        str = str.ToLower(this.List.Configuration.Culture);
        str = this.RemoveSpecialCharacters(str);

        return NumberRegex.Split(str)
            .Select(this.DelimitIfNumber)
            .Aggregate(String.Empty, (acc, item) => acc + item);
    }

    private string RemoveSpecialCharacters(string str)
    {
        str = str
            .Replace(":", Space)
            .Replace(";", Space)
            .Replace("!", Space)
            .Replace("?", Space)
            .Replace("¡", Space)
            .Replace("¿", Space)
            .Replace(".", Space)
            .Replace(",", Space)
            .Replace("-", Space)
            .Replace("–", Space)
            .Replace("—", Space)
            .Replace("_", Space)
            .Replace("*", Space)
            .Replace("+", Space)
            .Replace("=", Space)
            .Replace("#", Space)
            .Replace("$", Space)
            .Replace("%", Space)
            .Replace("&", Space)
            .Replace("'", Space)
            .Replace("’", Space)
            .Replace("`", Space)
            .Replace("\"", Space)
            .Replace("/", Space)
            .Replace("\\", Space)
            .Replace("(", Space)
            .Replace(")", Space)
            .Trim();

        return SpacesOrOtherCharactersRegex.Replace(str, Space);
    }

    private string DelimitIfNumber(string str) =>
        Int32.TryParse(str, out int result) ? this.DelimitNumber(result) : str;

    private string DelimitNumber(int num) =>
        $"{NumberDelimiter}{num}{NumberDelimiter}";
}

internal static class ListItemExtensions
{
    internal static IEnumerable<object> WithSplitNumbers(this string title) =>
        title.Split(NumberDelimiter, StringSplitOptions.RemoveEmptyEntries).Select(TryParseNumber);

    private static object TryParseNumber(string str) =>
        Int32.TryParse(str, out int result) ? result : str;
}
