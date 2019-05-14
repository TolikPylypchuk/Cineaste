using System.Collections.ObjectModel;
using System.Threading.Tasks;

using MovieList.Data.Models;
using MovieList.ViewModels.ListItems;

namespace MovieList.Services
{
    public interface IMovieService
    {
        Task<ObservableCollection<ListItem>> LoadListAsync();

        Task SaveMovieAsync(Movie movie);
        Task ToggleWatchedAsync(ListItem item);
        Task ToggleReleasedAsync(MovieListItem item);
    }
}
