using System.Threading.Tasks;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface IMovieService
    {
        Task SaveAsync(Movie movie);
        Task DeleteAsync(Movie movie);
    }
}
