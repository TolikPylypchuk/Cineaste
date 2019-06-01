using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MovieList.Config;
using MovieList.Data;
using MovieList.Data.Models;
using MovieList.ViewModels.ListItems;

namespace MovieList.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly MovieContext context;
        private readonly Configuration config;

        public MovieService(MovieContext context, IOptions<Configuration> config)
        {
            this.context = context;
            this.config = config.Value;
        }

        public async Task<List<ListItem>> LoadListAsync()
        {
            var movies = await context.Movies
                .Include(context.GetIncludePaths(typeof(Movie)))
                .ToListAsync();

            var series = await context.Series
                .Include(context.GetIncludePaths(typeof(Series)))
                .ToListAsync();

            var movieSeries = await context.MovieSeries
                .Include(context.GetIncludePaths(typeof(MovieSeries)))
                .ToListAsync();

            return movies
                .Select(movie => new MovieListItem(movie, this.config))
                .Cast<ListItem>()
                .Union(series.Select(series => new SeriesListItem(series, this.config)))
                .Union(movieSeries
                    .Where(series => series.Title != null)
                    .Select(series => new MovieSeriesListItem(series, this.config)))
                .OrderBy(item => item)
                .ToList();
        }

        public async Task SaveMovieAsync(Movie movie)
        {
            if (movie.Id == default)
            {
                this.context.Attach(movie.Kind);
                this.context.Add(movie);
            } else
            {
                this.context.Entry(movie).State = EntityState.Modified;

                foreach (var title in movie.Titles)
                {
                    if (title.Id == default)
                    {
                        this.context.Add(title);
                    } else
                    {
                        this.context.Entry(title).State = EntityState.Modified;
                    }
                }
            }

            await this.context.SaveChangesAsync();
        }

        public Task SaveSeriesAsync(Series series)
        {
            throw new NotImplementedException();
        }

        public async Task ToggleWatchedAsync(ListItem item)
        {
            switch (item)
            {
                case MovieListItem movieItem:
                    movieItem.Movie.IsWatched = !movieItem.Movie.IsWatched;
                    this.context.Attach(movieItem.Movie).State = EntityState.Modified;
                    await this.context.SaveChangesAsync();
                    break;
                case SeriesListItem seriesItem:
                    seriesItem.Series.IsWatched = !seriesItem.Series.IsWatched;
                    this.context.Attach(seriesItem.Series).State = EntityState.Modified;
                    await this.context.SaveChangesAsync();
                    break;
            }
        }

        public async Task ToggleReleasedAsync(MovieListItem item)
        {
            item.Movie.IsReleased = !item.Movie.IsReleased;
            this.context.Attach(item.Movie).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteAsync<TEntity>(TEntity entity)
            where TEntity : EntityBase
        {
            this.context.Attach(entity).State = EntityState.Deleted;
            await this.context.SaveChangesAsync();
        }
    }
}
