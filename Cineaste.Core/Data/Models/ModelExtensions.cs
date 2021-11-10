namespace Cineaste.Core.Data.Models;

public static class ModelExtensions
{
    public static string GetNumberToDisplay(this FranchiseEntry? entry) =>
        entry != null
            ? entry.ParentFranchise.IsLooselyConnected
                ? $"({entry.DisplayNumber})"
                : entry.DisplayNumber?.ToString() ?? NoDisplayNumberPlaceholder
            : String.Empty;

    public static string AsDisplayNumber(this int? number, bool inParentheses) =>
        number != null
            ? inParentheses ? $"({number})" : number.ToString() ?? NoDisplayNumberPlaceholder
            : NoDisplayNumberPlaceholder;

    public static string GetActiveColor(this Movie movie) =>
        movie.IsWatched
            ? movie.Kind.ColorForWatchedMovie
            : movie.IsReleased
                ? movie.Kind.ColorForNotWatchedMovie
                : movie.Kind.ColorForNotReleasedMovie;

    public static string GetActiveColor(this Series series) =>
        series.WatchStatus != SeriesWatchStatus.NotWatched
            ? series.Kind.ColorForWatchedSeries
            : series.ReleaseStatus != SeriesReleaseStatus.NotStarted
                ? series.Kind.ColorForNotWatchedSeries
                : series.Kind.ColorForNotReleasedSeries;

    public static string GetActiveColor(this Franchise franchise)
    {
        var firstEntry = franchise.GetFirstEntry();
        return firstEntry != null
            ? firstEntry.Movie?.GetActiveColor() ?? firstEntry.Series!.GetActiveColor()
            : String.Empty;
    }

    public static Title? GetTitle(this Franchise franchise) =>
        franchise.Titles.Count > 0 ? franchise.Title! : franchise.GetFirstEntry()?.GetTitle();

    public static Title? GetOriginalTitle(this Franchise franchise) =>
        franchise.Titles.Count > 0
            ? franchise.OriginalTitle!
            : franchise.GetFirstEntry()?.GetOriginalTitle();

    public static Title? GetListTitle(this Franchise franchise) =>
        franchise.ShowTitles ? franchise.Title! : franchise.GetFirstEntry()?.GetTitle();

    public static Title? GetListOriginalTitle(this Franchise franchise) =>
        franchise.ShowTitles ? franchise.OriginalTitle! : franchise.GetFirstEntry()?.GetOriginalTitle();

    public static Title? GetTitle(this FranchiseEntry entry) =>
        entry.Movie?.Title ?? entry.Series?.Title ?? entry.Franchise!.GetTitle();

    public static Title? GetOriginalTitle(this FranchiseEntry entry) =>
        entry.Movie?.OriginalTitle ?? entry.Series?.OriginalTitle ?? entry.Franchise!.GetOriginalTitle();

    public static FranchiseEntry? GetFirstEntry(this Franchise franchise)
    {
        var firstEntry = franchise.Entries.OrderBy(entry => entry.SequenceNumber).FirstOrDefault();
        return firstEntry != null
            ? firstEntry.Franchise == null ? firstEntry : firstEntry.Franchise.GetFirstEntry()
            : null;
    }

    public static FranchiseEntry? GetLastEntry(this Franchise franchise)
    {
        var lastEntry = franchise.Entries.OrderByDescending(entry => entry.SequenceNumber).FirstOrDefault();
        return lastEntry != null
            ? lastEntry.Franchise == null ? lastEntry : lastEntry.Franchise.GetLastEntry()
            : null;
    }

    public static Franchise GetRootSeries(this Franchise franchise) =>
        franchise.Entry == null ? franchise : franchise.Entry.ParentFranchise.GetRootSeries();

    public static bool IsDescendantOf(this Franchise? franchise, Franchise potentialAncestor) =>
        franchise != null &&
            (franchise.Id == potentialAncestor.Id ||
                (franchise.Entry?.ParentFranchise.IsDescendantOf(potentialAncestor) ?? false));

    public static bool IsStrictDescendantOf(this Franchise? franchise, Franchise potentialAncestor) =>
        franchise != potentialAncestor && franchise.IsDescendantOf(potentialAncestor);

    public static IEnumerable<Franchise> GetAllAncestors(this Franchise? series)
    {
        if (series == null)
        {
            yield break;
        }

        if (series.Entry != null)
        {
            foreach (var ancestor in series.Entry.ParentFranchise.GetAllAncestors())
            {
                yield return ancestor;
            }
        }

        yield return series;
    }

    public static (Franchise, Franchise) GetDistinctAncestors(this Franchise series1, Franchise series2) =>
        series1.GetAllAncestors()
            .Zip(series2.GetAllAncestors(), (a, b) => (Series1: a, Series2: b))
            .First(ancestors => ancestors.Series1.Id != ancestors.Series2.Id);

    public static int GetStartYear(this Franchise franchise) =>
        franchise.GetFirstEntry()?.GetStartYear() ?? default;

    public static int GetStartYear(this FranchiseEntry entry) =>
        entry.Movie?.Year ?? entry.Series?.StartYear ?? entry.Franchise!.GetStartYear();

    public static int GetEndYear(this Franchise franchise) =>
        franchise.GetLastEntry()?.GetEndYear() ?? default;

    public static int GetEndYear(this FranchiseEntry entry) =>
        entry.Movie?.Year ?? entry.Series?.EndYear ?? entry.Franchise!.GetEndYear();

    public static string GetYears(this FranchiseEntry entry) =>
        entry.Movie != null
            ? entry.Movie.Year.ToString()
            : entry.Series != null
                ? entry.Series.GetYears()
                : entry.Franchise!.GetYears();

    public static string GetYears(this Series series)
    {
        int startYear = series.StartYear;
        int endYear = series.EndYear;
        return startYear == endYear ? startYear.ToString() : $"{startYear}-{endYear}";
    }

    public static string GetYears(this Franchise franchise)
    {
        int startYear = franchise.GetStartYear();
        int endYear = franchise.GetEndYear();
        return startYear == default
            ? "-"
            : startYear == endYear ? startYear.ToString() : $"{startYear}-{endYear}";
    }

    public static ListItem ToListItem(this FranchiseEntry entry) =>
        entry.Movie != null
            ? new MovieListItem(entry.Movie)
            : entry.Series != null
                ? (ListItem)new SeriesListItem(entry.Series)
                : new FranchiseListItem(entry.Franchise!);
}
