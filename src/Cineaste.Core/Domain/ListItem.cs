using System.Text.RegularExpressions;

namespace Cineaste.Core.Domain;

public sealed partial class ListItem : Entity<ListItem>
{
    private const string Space = " ";

    private static readonly Regex NumberRegex = CreateNumberRegex();
    private static readonly Regex SpacesRegex = CreateSpacesRegex();

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

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private ListItem(Id<ListItem> id)
        : base(id)
    {
        this.List = null!;
        this.NormalizedTitle = String.Empty;
        this.NormalizedOriginalTitle = String.Empty;
        this.NormalizedShortTitle = String.Empty;
        this.NormalizedShortOriginalTitle = String.Empty;
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
    }

    private string CreateFullTitle(FranchiseItem? item, string title) =>
        item is null
            ? title
            : this.CreateFullTitle(
                item.ParentFranchise.FranchiseItem,
                $"{this.Normalize(item.ParentFranchise.ActualTitle?.Name)}:{item.SequenceNumber:D5}:{title}");

    private string Normalize(string? str) =>
        String.IsNullOrEmpty(str)
            ? String.Empty
            : NumberRegex.Split(this.RemoveSpecialCharacters(str))
                .Select(this.NormalizeNumbers)
                .Aggregate(String.Empty, (acc, item) => acc + item);

    private string RemoveSpecialCharacters(string str) =>
        SpacesRegex.Replace(
            str.ToLower(this.List.Configuration.Culture)
                .Replace(":", Space)
                .Replace(";", Space)
                .Replace("!", Space)
                .Replace("?", Space)
                .Replace("¡", Space)
                .Replace("¿", Space)
                .Replace(".", Space)
                .Replace(",", Space)
                .Replace(" - ", Space)
                .Replace(" – ", Space)
                .Replace(" — ", Space)
                .Replace("-", Space)
                .Replace("_", Space)
                .Replace("'", Space)
                .Replace("’", Space)
                .Replace("`", Space)
                .Replace("\"", Space)
                .Replace("/", Space)
                .Replace("\\", Space),
            Space);

    private object NormalizeNumbers(string str) =>
        Int32.TryParse(str, out int result) ? $"{result:D10}" : str;

    [GeneratedRegex("([0-9]+)", RegexOptions.Compiled)]
    private static partial Regex CreateNumberRegex();

    [GeneratedRegex("\\s+", RegexOptions.Compiled)]
    private static partial Regex CreateSpacesRegex();
}
