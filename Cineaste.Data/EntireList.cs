using System.Collections.Generic;

using Cineaste.Data.Models;

namespace Cineaste.Data
{
    public sealed record EntireList(
        IEnumerable<Movie> Movies,
        IEnumerable<Series> Series,
        IEnumerable<Franchise> Franchise);
}
