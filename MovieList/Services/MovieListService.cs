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
                    .ToList()
                    .Select(movie => new MovieListItem(movie))
                    .Cast<ListItem>()
                    .Union(context.Series
                        .ToList()
                        .Select(series => new SeriesListItem(series)))
                    .Union(context.MovieSeries
                        .ToList()
                        .Where(series => series.Title != null)
                        .Select(series => new MovieSeriesListItem(series)))
                    .OrderBy(item => item));
    }
}
