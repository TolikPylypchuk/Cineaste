using System.Data;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using Microsoft.Data.Sqlite;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal class MovieSeriesService : EntityServiceBase<MovieSeries>
    {
        public MovieSeriesService(string file)
            : base(file)
        { }

        protected override async Task InsertAsync(
            MovieSeries movieSeries,
            SqliteConnection connection,
            IDbTransaction transaction)
        {
            movieSeries.Id = await connection.InsertAsync(movieSeries, transaction);

            foreach (var title in movieSeries.Titles)
            {
                title.MovieSeriesId = movieSeries.Id;
                title.Id = await connection.InsertAsync(title, transaction);
            }

            if (movieSeries.Entry != null)
            {
                var entry = movieSeries.Entry;
                entry.Id = await connection.InsertAsync(entry, transaction);
                entry.MovieSeriesId = movieSeries.Id;
                entry.ParentSeries.Entries.Add(entry);
            }
        }

        protected override async Task UpdateAsync(
            MovieSeries movieSeries,
            SqliteConnection connection,
            IDbTransaction transaction)
        {
            await connection.UpdateAsync(movieSeries, transaction);

            await new DependentEntityUpdater(connection, transaction).UpdateDependentEntitiesAsync(
                movieSeries,
                movieSeries.Titles,
                title => title.MovieSeriesId,
                (title, movieSeriesId) => title.MovieSeriesId = movieSeriesId);

            await new DependentEntityUpdater(connection, transaction).UpdateDependentEntitiesAsync(
                movieSeries,
                movieSeries.Entries,
                entry => entry.ParentSeriesId,
                (entry, movieSeriesId) => entry.ParentSeriesId = movieSeriesId);

            if (movieSeries.Entry != null)
            {
                await connection.UpdateAsync(movieSeries.Entry, transaction);
            }
        }

        protected override async Task DeleteAsync(
            MovieSeries movieSeries,
            SqliteConnection connection,
            IDbTransaction transaction)
        {
            await connection.DeleteAsync(movieSeries.Titles, transaction);
            await connection.DeleteAsync(movieSeries.Entries, transaction);

            if (movieSeries.Entry != null)
            {
                await this.DeleteMovieSeriesEntryAsync(movieSeries.Entry, connection, transaction);
            }

            await connection.DeleteAsync(movieSeries, transaction);
        }
    }
}
