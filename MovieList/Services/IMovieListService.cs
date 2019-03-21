using System.Collections.ObjectModel;

using MovieList.ViewModels.ListItems;

namespace MovieList.Services
{
    public interface IMovieListService
    {
        ObservableCollection<ListItem> LoadAllItems();
    }
}
