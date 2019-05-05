using System;
using System.Windows.Media;

using MovieList.Config;
using MovieList.Data.Models;

namespace MovieList.ViewModels.ListItems
{
    public abstract class ListItem :
        ViewModelBase,
        IEquatable<ListItem>,
        IComparable<ListItem>,
        IComparable<MovieListItem>,
        IComparable<SeriesListItem>,
        IComparable<MovieSeriesListItem>
    {
        private string displayNumber;
        private string title;
        private string originalTitle;
        private string year;
        private Color color;

        protected ListItem(MovieSeriesEntry? entry, string title, string originalTitle, string year, Color color)
        {
            this.DisplayNumber = entry.GetDisplayNumber();
            this.Title = title;
            this.OriginalTitle = originalTitle;
            this.Year = year;
            this.Color = color;
        }

        public string DisplayNumber
        {
            get => this.displayNumber;
            set
            {
                this.displayNumber = value;
                this.OnPropertyChanged();
            }
        }

        public string Title
        {
            get => this.title;
            set
            {
                this.title = value;
                this.OnPropertyChanged();
            }
        }

        public string OriginalTitle
        {
            get => this.originalTitle;
            set
            {
                this.originalTitle = value;
                this.OnPropertyChanged();
            }
        }

        public string Year
        {
            get => this.year;
            set
            {
                this.year = value;
                this.OnPropertyChanged();
            }
        }
        public Color Color
        {
            get => this.color;
            set
            {
                this.color = value;
                this.OnPropertyChanged();
            }
        }

        public override bool Equals(object obj)
            => obj is ListItem item && this.Equals(item);

        public bool Equals(ListItem other)
            => other != null &&
                this.DisplayNumber == other.DisplayNumber &&
                this.Title == other.DisplayNumber &&
                this.OriginalTitle == other.OriginalTitle &&
                this.Year == other.Year &&
                this.Color == other.Color;

        public override int GetHashCode()
            => Util.GetHashCode(this.DisplayNumber, this.Title, this.OriginalTitle, this.Year, this.Color);

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

        public abstract void UpdateColor(Configuration? config);

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

        private int CompareTitleOrYear(ListItem item)
        {
            int result = this.Title.CompareTo(item.Title);
            return result != 0 ? result : this.Year.CompareTo(item.Year);
        }
    }
}
