using System;

using MovieList.Data.Models;

namespace MovieList.ListItems
{
    public class MovieSeriesListItem : ListItem
    {
        public MovieSeriesListItem(MovieSeries movieSeries)
            : base(
                $"MS-{movieSeries.Id}",
                null,
                movieSeries.Title != null ? $"{movieSeries.Title.Name}:" : String.Empty,
                movieSeries.OriginalTitle != null ? $"{movieSeries.OriginalTitle.Name}:" : String.Empty,
                String.Empty,
                movieSeries.GetActiveColor())
            => this.MovieSeries = movieSeries;

        public MovieSeries MovieSeries { get; }

        public override void Refresh()
            => this.SetProperties(
                this.MovieSeries.Entry,
                this.MovieSeries.Title != null ? $"{this.MovieSeries.Title.Name}:" : String.Empty,
                this.MovieSeries.OriginalTitle != null ? $"{this.MovieSeries.OriginalTitle.Name}:" : String.Empty,
                String.Empty,
                this.MovieSeries.GetActiveColor());
    }
}
