using System;
using System.Windows.Media;

using MovieList.Data.Models;

namespace MovieList.ViewModels.ListItems
{
    public abstract class ListItem :
        IComparable<ListItem>,
        IComparable<MovieListItem>,
        IComparable<SeriesListItem>,
        IComparable<MovieSeriesListItem>
    {
        protected ListItem(MovieSeriesEntry? entry, string title, string originalTitle, string year, Color color)
        {
            this.DisplayNumber = entry != null ? GetDisplayNumber(entry) : String.Empty;
            this.Title = title;
            this.OriginalTitle = originalTitle;
            this.Year = year;
            this.Color = color;
        }

        public string DisplayNumber { get; }
        public string Title { get; }
        public string OriginalTitle { get; }
        public string Year { get; }
        public Color Color { get; }

        public int CompareTo(ListItem other)
            => other switch
            {
                MovieListItem item => this.CompareTo(item),
                SeriesListItem item => this.CompareTo(item),
                MovieSeriesListItem item => this.CompareTo(item),
                _ => throw new NotSupportedException("Unknown list item type."),
            };

        public abstract int CompareTo(MovieListItem other);
        public abstract int CompareTo(SeriesListItem other);
        public abstract int CompareTo(MovieSeriesListItem other);

        protected int CompareToEntry(ListItem item, MovieSeriesEntry? thisEntry, MovieSeriesEntry? otherEntry)
            => (thisEntry, otherEntry) switch
        {
            (null, null) => this.CompareTitleOrYear(item),
            (var entry, null) => new MovieSeriesListItem(entry.MovieSeries).CompareTo(item),
            (null, var entry) => this.CompareTo(new MovieSeriesListItem(entry.MovieSeries)),
            (var entry1, var entry2) => entry1.MovieSeriesId == entry2.MovieSeriesId
                ? entry1.OrdinalNumber.CompareTo(entry2.OrdinalNumber)
                : new MovieSeriesListItem(entry1.MovieSeries)
                    .CompareTo(new MovieSeriesListItem(entry2.MovieSeries))
        };

        private static string GetDisplayNumber(MovieSeriesEntry entry)
            => entry.MovieSeries.IsLooselyConnected
                ? $"({entry.OrdinalNumber})"
                : entry.DisplayNumber?.ToString() ?? "-";

        private int CompareTitleOrYear(ListItem item)
        {
            int result = this.Title.CompareTo(item.Title);
            return result != 0 ? result : this.Year.CompareTo(item.Year);
        }
    }
}
