using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data
{
    public sealed class MovieList
    {
        public MovieList(IEnumerable<Movie> movies, IEnumerable<Series> series, IEnumerable<MovieSeries> movieSeries)
        {
            this.Movies = movies;
            this.Series = series;
            this.MovieSeries = movieSeries;
        }

        public IEnumerable<Movie> Movies { get; }
        public IEnumerable<Series> Series { get; }
        public IEnumerable<MovieSeries> MovieSeries { get; }
    }
}
