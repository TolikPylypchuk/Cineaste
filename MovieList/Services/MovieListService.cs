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
        private readonly UIConfig uiConfig;

        public MovieListService(MovieContext context, IOptions<UIConfig> uiConfig)
        {
            this.context = context;
            this.uiConfig = uiConfig.Value;
        }

        public async Task<ObservableCollection<ListItem>> LoadAllItemsAsync()
        {
            var movies = await context.Movies.ToListAsync();
            var series = await context.Series.ToListAsync();
            var movieSeries = await context.MovieSeries.ToListAsync();

            return new ObservableCollection<ListItem>(
                movies
                    .Select(movie => new MovieListItem(movie, this.uiConfig))
                    .Cast<ListItem>()
                    .Union(series.Select(series => new SeriesListItem(series, this.uiConfig)))
                    .Union(movieSeries
                        .Where(series => series.Title != null)
                        .Select(series => new MovieSeriesListItem(series, this.uiConfig)))
                    .OrderBy(item => item));
        }
    }
}
