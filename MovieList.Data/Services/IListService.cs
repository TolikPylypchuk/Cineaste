using System.Collections.Generic;
using System.Threading.Tasks;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface IListService
    {
        Task<(IEnumerable<Movie> Movies, IEnumerable<Series> Series, IEnumerable<MovieSeries> MovieSeries)> GetListAsync(IList<Kind> kinds);
    }
}
