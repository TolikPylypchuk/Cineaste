using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MovieList.Data;
using MovieList.ViewModels.ListItems;

namespace MovieList.Services
{
    public class MovieListService : IMovieListService
    {
        private readonly MovieContext context;

        public MovieListService(MovieContext context)
            => this.context = context;

        public async Task<ObservableCollection<ListItem>> LoadAllItemsAsync()
        {
            var movies = await context.Movies.ToListAsync();
            var series = await context.Series.ToListAsync();
            var movieSeries = await context.MovieSeries.ToListAsync();

            return new ObservableCollection<ListItem>(
                movies
                    .Select(movie => new MovieListItem(movie))
                    .Cast<ListItem>()
                    .Union(series.Select(series => new SeriesListItem(series)))
                    .Union(movieSeries
                        .Where(series => series.Title != null)
                        .Select(series => new MovieSeriesListItem(series)))
                    .OrderBy(item => item));
        }
    }
}
