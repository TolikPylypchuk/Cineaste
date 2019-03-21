using System.Collections.ObjectModel;
using System.Linq;

using MovieList.Data;
using MovieList.ViewModels.ListItems;

namespace MovieList.Services
{
    public class MovieListService : IMovieListService
    {
        private readonly MovieContext context;

        public MovieListService(MovieContext context)
            => this.context = context;

        public ObservableCollection<ListItem> LoadAllItems()
            => new ObservableCollection<ListItem>(
                context.Movies
                    .Select(movie => new MovieListItem(movie))
                    .Cast<ListItem>()
                    .Union(context.Series.Select(series => new SeriesListItem(series)))
                    .OrderBy(item => item));
    }
}
