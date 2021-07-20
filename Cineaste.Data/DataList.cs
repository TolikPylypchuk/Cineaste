using System.Collections.Generic;

using Cineaste.Data.Models;

namespace Cineaste.Data
{
    public sealed record DataList(
        IEnumerable<Movie> Movies,
        IEnumerable<Series> Series,
        IEnumerable<Franchise> Franchises);
}
