using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data
{
    public sealed record WholeList(
        IEnumerable<Movie> Movies,
        IEnumerable<Series> Series,
        IEnumerable<Franchise> Franchise);
}
