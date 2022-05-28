namespace Cineaste.Server.Domain;

public static class Extensions
{
    public static string GetDisplayNumber(this FranchiseItem? item)
    {
        if (item is null)
        {
            return String.Empty;
        }

        if (!item.ShouldDisplayNumber)
        {
            return "-";
        }

        int num = item.ParentFranchise
            .Children
            .OrderBy(child => child.SequenceNumber)
            .Where(child => child.ShouldDisplayNumber)
            .TakeWhile(child => child.SequenceNumber <= item.SequenceNumber)
            .Select((child, index) => index)
            .Last() + 1;

        return item.ParentFranchise.IsLooselyConnected ? $"({num})" : num.ToString();
    }

    public static Color GetActiveColor(this Movie movie) =>
        movie.IsWatched
            ? movie.Kind.WatchedColor
            : movie.IsReleased ? movie.Kind.NotWatchedColor : movie.Kind.NotReleasedColor;

    public static Color GetActiveColor(this Series series) =>
        series.WatchStatus != SeriesWatchStatus.NotWatched
            ? series.Kind.WatchedColor
            : series.ReleaseStatus != SeriesReleaseStatus.NotStarted
                ? series.Kind.NotWatchedColor
                : series.Kind.NotReleasedColor;

    public static Color? GetActiveColor(this Franchise franchise) =>
        franchise.GetFirstChild()?.Select(
            movie => movie.GetActiveColor(),
            series => series.GetActiveColor(),
            franchise => franchise.GetActiveColor());

    public static FranchiseItem? GetFirstChild(this Franchise franchise)
    {
        var item = franchise.Children.OrderBy(item => item.SequenceNumber).FirstOrDefault();
        return item is not null
            ? item.Franchise == null ? item : item.Franchise.GetFirstChild()
            : null;
    }

    public static FranchiseItem? GetLastChild(this Franchise franchise)
    {
        var item = franchise.Children.OrderByDescending(item => item.SequenceNumber).FirstOrDefault();
        return item is not null
            ? item.Franchise == null ? item : item.Franchise.GetLastChild()
            : null;
    }

    public static int GetStartYear(this FranchiseItem item) =>
        item.Select(
            movie => movie.Year,
            series => series.StartYear,
            franchise => franchise.GetFirstChild()?.GetStartYear() ?? 0);

    public static int GetEndYear(this FranchiseItem item) =>
        item.Select(
            movie => movie.Year,
            series => series.EndYear,
            franchise => franchise.GetLastChild()?.GetEndYear() ?? 0);
}
