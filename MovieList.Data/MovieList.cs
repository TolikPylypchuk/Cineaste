using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data
{
    public sealed class MovieList
    {
        public MovieList(IEnumerable<Movie> movies, IEnumerable<Series> series, IEnumerable<Franchise> franchise)
        {
            this.Movies = movies;
            this.Series = series;
            this.Franchise = franchise;
        }

        public IEnumerable<Movie> Movies { get; }
        public IEnumerable<Series> Series { get; }
        public IEnumerable<Franchise> Franchise { get; }
    }
}
