namespace Cineaste.Core.Domain;

public static class DomainExtensions
{
    extension(ListItem item)
    {
        public T Select<T>(Func<Movie, T> movieFunc, Func<Series, T> seriesFunc, Func<Franchise, T> franchiseFunc)
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

        public void Do(Action<Movie> movieAction, Action<Series> seriesAction, Action<Franchise> franchiseAction)
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
    }

    extension(FranchiseItem item)
    {
        public int StartYear =>
            item.Select(
                movie => movie.Year,
                series => series.StartYear,
                franchise => franchise.FirstChild?.StartYear ?? 0);

        public int EndYear =>
            item.Select(
                movie => movie.Year,
                series => series.EndYear,
                franchise => franchise.LastChild?.EndYear ?? 0);

        public T Select<T>(Func<Movie, T> movieFunc, Func<Series, T> seriesFunc, Func<Franchise, T> franchiseFunc)
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

        public void Do(Action<Movie> movieAction, Action<Series> seriesAction, Action<Franchise> franchiseAction)
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

        public void SetListItemProperties() =>
            item.Do(
                movie => movie.ListItem?.SetProperties(movie),
                series => series.ListItem?.SetProperties(series),
                franchise => franchise.SetListItemProperties());
    }

    extension(FranchiseItem? item)
    {
        public bool IsFirst =>
            item is not null && item.SequenceNumber == FirstSequenceNumber;

        public bool IsLast =>
            item is not null && item.SequenceNumber == item.ParentFranchise.Children.Count;
    }

    extension(Movie movie)
    {
        public Color ActiveColor =>
            movie.IsWatched
                ? movie.Kind.WatchedColor
                : movie.IsReleased ? movie.Kind.NotWatchedColor : movie.Kind.NotReleasedColor;
    }

    extension(Series series)
    {
        public Color ActiveColor =>
            series.WatchStatus != SeriesWatchStatus.NotWatched
                ? series.Kind.WatchedColor
                : series.ReleaseStatus != SeriesReleaseStatus.NotStarted
                    ? series.Kind.NotWatchedColor
                    : series.Kind.NotReleasedColor;
    }

    extension(Franchise franchise)
    {
        public Color ActiveColor
        {
            get
            {
                var kind = franchise.ActiveKind;

                return franchise.IsWatched
                    ? kind.WatchedColor
                    : franchise.IsReleased
                        ? kind.NotWatchedColor
                        : kind.NotReleasedColor;
            }
        }

        public IKind ActiveKind =>
            franchise.KindSource == FranchiseKindSource.Series
                ? franchise.SeriesKind
                : franchise.MovieKind;

        public bool IsWatched =>
            franchise.FirstChild?.Select(
                movie => movie.IsWatched,
                series => series.WatchStatus != SeriesWatchStatus.NotWatched,
                franchise => franchise.IsWatched)
                ?? true;

        public bool IsReleased =>
            franchise.FirstChild?.Select(
                movie => movie.IsReleased,
                series => series.ReleaseStatus != SeriesReleaseStatus.NotStarted,
                franchise => franchise.IsReleased)
                ?? true;

        public FranchiseItem? FirstChild
        {
            get
            {
                var item = franchise.Children.OrderBy(item => item.SequenceNumber).FirstOrDefault();
                return item is not null
                    ? item.Franchise == null ? item : item.Franchise.FirstChild
                    : null;
            }
        }

        public FranchiseItem? LastChild
        {
            get
            {
                var item = franchise.Children.OrderByDescending(item => item.SequenceNumber).FirstOrDefault();
                return item is not null
                    ? item.Franchise == null ? item : item.Franchise.LastChild
                    : null;
            }
        }

        public void SetListItemProperties()
        {
            franchise.ListItem?.SetProperties(franchise);

            foreach (var item in franchise.Children)
            {
                item.SetListItemProperties();
            }
        }
    }
}
