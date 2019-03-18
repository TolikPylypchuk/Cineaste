using System;
using System.Windows.Media;

using MovieList.Data.Models;

namespace MovieList.ViewModels.ListItems
{
    public abstract class ListItemBase
    {
        protected ListItemBase(MovieSeriesEntry? entry, string title, string originalTitle, string year, Color color)
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

        private static string GetOrdinalNumber(MovieSeriesEntry entry)
            => entry.MovieSeries.IsLooselyConnected
                ? $"({entry.OrdinalNumber})"
                : entry.ShowOrdinalNumber ? entry.OrdinalNumber.ToString() : "-";
    }
}
