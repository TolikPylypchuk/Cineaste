using System.Collections.Generic;
using System.Threading.Tasks;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface IListService
    {
        Task<(IEnumerable<Movie>, IEnumerable<Series>, IEnumerable<MovieSeries>)> GetListAsync();
    }
}
