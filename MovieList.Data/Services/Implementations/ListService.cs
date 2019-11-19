using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal class ListService : ServiceBase, IListService
    {
        public ListService(string file)
            : base(file)
        { }

        [LogException]
        public async Task<(IEnumerable<Movie> Movies, IEnumerable<Series> Series, IEnumerable<MovieSeries> MovieSeries)> GetListAsync(IList<Kind> kinds)
        {
            this.Log().Debug("Getting the full list of movies, series and movie series.");

            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            var titles = await connection.GetAllAsync<Title>(transaction).ToListAsync();
            var entries = await connection.GetAllAsync<MovieSeriesEntry>(transaction).ToListAsync();

            var seasons = await connection.GetAllAsync<Season>(transaction).ToListAsync();
            var periods = await connection.GetAllAsync<Period>(transaction).ToListAsync();
            var specialEpisodes = await connection.GetAllAsync<SpecialEpisode>(transaction).ToListAsync();

            var movies = (await connection.GetAllAsync<Movie>(transaction).ToListAsync())
                .Join(kinds)
                .Join(titles)
                .Join(entries)
                .ToList();

            var series = (await connection.GetAllAsync<Series>(transaction).ToListAsync())
                .Join(kinds)
                .Join(titles)
                .Join(seasons.Join(periods).Join(titles))
                .Join(specialEpisodes.Join(titles))
                .Join(entries)
                .ToList();

            var movieSeries = (await connection.GetAllAsync<MovieSeries>(transaction).ToListAsync())
                .Join(titles)
                .Join(entries)
                .ToList();

            await transaction.CommitAsync();

            return (movies, series, movieSeries);
        }
    }
}
