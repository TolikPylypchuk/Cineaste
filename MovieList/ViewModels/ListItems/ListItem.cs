using System;
using System.Windows.Media;

using MovieList.Data.Models;

namespace MovieList.ViewModels.ListItems
{
    public abstract class ListItem : IComparable<ListItem>
    {
        protected ListItem(MovieSeriesEntry? entry, string title, string originalTitle, string year, Color color)
        {
            this.OrdinalNumber = entry != null ? GetOrdinalNumber(entry) : String.Empty;
            this.Title = title;
            this.OriginalTitle = originalTitle;
            this.Year = year;
            this.Color = color;
        }

        public string OrdinalNumber { get; }
        public string Title { get; }
        public string OriginalTitle { get; }
        public string Year { get; }
        public Color Color { get; }

        public abstract string SelectTitleToCompare();

        public int CompareTo(ListItem other)
            => this.SelectTitleToCompare().CompareTo(other.SelectTitleToCompare());

        private static string GetOrdinalNumber(MovieSeriesEntry entry)
            => entry.MovieSeries.IsLooselyConnected
                ? $"({entry.OrdinalNumber})"
                : entry.ShowOrdinalNumber ? entry.OrdinalNumber.ToString() : "-";
    }
}
