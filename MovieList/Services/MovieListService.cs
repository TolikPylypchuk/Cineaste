using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MovieList.Config;
using MovieList.Data;
using MovieList.ViewModels.ListItems;

namespace MovieList.Services
{
    public class MovieListService : IMovieListService
    {
        private readonly MovieContext context;
        private readonly Configuration config;

        public MovieListService(MovieContext context, IOptions<Configuration> config)
        {
            this.context = context;
            this.config = config.Value;
        }

        public async Task<ObservableCollection<ListItem>> LoadAllItemsAsync()
        {
            var movies = await context.Movies.ToListAsync();
            var series = await context.Series.ToListAsync();
            var movieSeries = await context.MovieSeries.ToListAsync();

            return new ObservableCollection<ListItem>(
                movies
                    .Select(movie => new MovieListItem(movie, this.config))
                    .Cast<ListItem>()
                    .Union(series.Select(series => new SeriesListItem(series, this.config)))
                    .Union(movieSeries
                        .Where(series => series.Title != null)
                        .Select(series => new MovieSeriesListItem(series, this.config)))
                    .OrderBy(item => item));
        }
    }
}
