using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using Microsoft.Data.Sqlite;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class MovieService : ServiceBase, IMovieService
    {
        public MovieService(string fileName)
            : base(fileName)
        { }

        public Task SaveAsync(Movie movie)
            => Task.CompletedTask;

        public async Task DeleteAsync(Movie movie)
        {
            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            await connection.DeleteAsync(movie.Titles, transaction);

            if (movie.Entry != null)
            {
                await this.DeleteMovieSeriesEntryAsync(movie.Entry, connection, transaction);
            }

            await connection.DeleteAsync(movie, transaction);

            await transaction.CommitAsync();
        }

        private async Task DeleteMovieSeriesEntryAsync(
            MovieSeriesEntry movieSeriesEntry,
            SqliteConnection connection,
            IDbTransaction transaction)
        {
            await connection.DeleteAsync(movieSeriesEntry, transaction);

            var movieSeries = movieSeriesEntry.ParentSeries;

            foreach (var entry in movieSeries.Entries
                .Where(entry => entry.SequenceNumber > movieSeriesEntry.SequenceNumber))
            {
                entry.SequenceNumber--;

                if (entry.DisplayNumber != null)
                {
                    entry.DisplayNumber--;
                }

                await connection.UpdateAsync(entry, transaction);
            }

            movieSeries.Entries.Remove(movieSeriesEntry);

            if (movieSeries.Entries.Count == 0)
            {
                await connection.DeleteAsync(movieSeries, transaction);

                if (movieSeries.Entry != null)
                {
                    await this.DeleteMovieSeriesEntryAsync(movieSeries.Entry, connection, transaction);
                }
            }
        }
    }
}
