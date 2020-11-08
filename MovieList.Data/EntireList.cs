using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data
{
    public sealed record EntireList(
        IEnumerable<Movie> Movies,
        IEnumerable<Series> Series,
        IEnumerable<Franchise> Franchise);
}
