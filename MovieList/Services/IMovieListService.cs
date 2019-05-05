using System.Collections.ObjectModel;
using System.Threading.Tasks;

using MovieList.ViewModels.ListItems;

namespace MovieList.Services
{
    public interface IMovieListService
    {
        Task<ObservableCollection<ListItem>> LoadAllItemsAsync();
        Task ToggleWatchedAsync(ListItem item);
        Task ToggleReleasedAsync(MovieListItem item);
    }
}
