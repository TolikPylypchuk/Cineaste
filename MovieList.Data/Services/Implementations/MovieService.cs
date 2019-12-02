using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using Microsoft.Data.Sqlite;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class MovieService : EntityServiceBase<Movie>
    {
        public MovieService(string fileName)
            : base(fileName)
        { }

        public override async Task SaveAsync(Movie movie)
        {
            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            if (movie.Id == default)
            {
                await this.InsertMovieAsync(movie, connection, transaction);
            } else
            {
                await this.UpdateMovieAsync(movie, connection, transaction);
            }

            await transaction.CommitAsync();
        }

        public override async Task DeleteAsync(Movie movie)
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

        private async Task InsertMovieAsync(Movie movie, SqliteConnection connection, IDbTransaction transaction)
        {
            movie.KindId = movie.Kind.Id;

            movie.Id = await connection.InsertAsync(movie, transaction);

            foreach (var title in movie.Titles)
            {
                title.MovieId = movie.Id;
                title.Id = await connection.InsertAsync(title, transaction);
            }

            if (movie.Entry != null)
            {
                var entry = movie.Entry;
                entry.Id = await connection.InsertAsync(entry, transaction);
                entry.MovieId = movie.Id;
                entry.ParentSeries.Entries.Add(entry);
            }
        }

        private async Task UpdateMovieAsync(Movie movie, SqliteConnection connection, IDbTransaction transaction)
        {
            await connection.UpdateAsync(movie, transaction);

            var dbTitles = (await connection.GetAllAsync<Title>(transaction))
                .Where(title => title.MovieId == movie.Id)
                .ToList();

            await connection.UpdateAsync(movie.Titles.Intersect(dbTitles, IdEqualityComparer<Title>.Instance), transaction);

            foreach (var title in movie.Titles.Except(dbTitles, IdEqualityComparer<Title>.Instance))
            {
                title.MovieId = movie.Id;
                title.Id = await connection.InsertAsync(title, transaction);
            }

            await connection.DeleteAsync(dbTitles.Except(movie.Titles, IdEqualityComparer<Title>.Instance), transaction);

            if (movie.Entry != null)
            {
                await connection.UpdateAsync(movie.Entry, transaction);
            }
        }
    }
}
