namespace Cineaste.Core;

public static class DomainExtensions
{
    public static T Select<T>(
        this ListItem item,
        Func<Movie, T> movieFunc,
        Func<Series, T> seriesFunc,
        Func<Franchise, T> franchiseFunc)
    {
        if (item.Movie is not null)
        {
            return movieFunc(item.Movie);
        } else if (item.Series is not null)
        {
            return seriesFunc(item.Series);
        } else if (item.Franchise is not null)
        {
            return franchiseFunc(item.Franchise);
        } else
        {
            throw new InvalidOperationException("Exactly one list item component must be non-null");
        }
    }

    public static T Select<T>(
        this FranchiseItem item,
        Func<Movie, T> movieFunc,
        Func<Series, T> seriesFunc,
        Func<Franchise, T> franchiseFunc)
    {
        if (item.Movie is not null)
        {
            return movieFunc(item.Movie);
        } else if (item.Series is not null)
        {
            return seriesFunc(item.Series);
        } else if (item.Franchise is not null)
        {
            return franchiseFunc(item.Franchise);
        } else
        {
            throw new InvalidOperationException("Exactly one franchise item component must be non-null");
        }
    }

    public static void Do(
        this ListItem item,
        Action<Movie> movieAction,
        Action<Series> seriesAction,
        Action<Franchise> franchiseAction)
    {
        if (item.Movie is not null)
        {
            movieAction(item.Movie);
        } else if (item.Series is not null)
        {
            seriesAction(item.Series);
        } else if (item.Franchise is not null)
        {
            franchiseAction(item.Franchise);
        } else
        {
            throw new InvalidOperationException("Exactly one list item component must be non-null");
        }
    }

    public static void Do(
        this FranchiseItem item,
        Action<Movie> movieAction,
        Action<Series> seriesAction,
        Action<Franchise> franchiseAction)
    {
        if (item.Movie is not null)
        {
            movieAction(item.Movie);
        } else if (item.Series is not null)
        {
            seriesAction(item.Series);
        } else if (item.Franchise is not null)
        {
            franchiseAction(item.Franchise);
        } else
        {
            throw new InvalidOperationException("Exactly one franchise item component must be non-null");
        }
    }

    public static bool IsFirst(this FranchiseItem? item) =>
        item is not null && item.SequenceNumber == 1;

    public static bool IsLast(this FranchiseItem? item) =>
        item is not null && item.SequenceNumber == item.ParentFranchise.Children.Count;

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
