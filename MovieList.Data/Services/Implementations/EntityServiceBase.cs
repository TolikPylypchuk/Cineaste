using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using Microsoft.Data.Sqlite;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal abstract class EntityServiceBase<TEntity> : ServiceBase, IEntityService<TEntity>
        where TEntity : EntityBase
    {
        protected EntityServiceBase(string file)
            : base(file)
        { }

        public Task SaveAsync(TEntity entity)
            => this.WithTransactionAsync(
                (connection, transaction) => entity.Id == default
                    ? this.InsertAsync(entity, connection, transaction)
                    : this.UpdateAsync(entity, connection, transaction));

        public Task DeleteAsync(TEntity entity)
            => this.WithTransactionAsync((connection, transaction) => this.DeleteAsync(entity, connection, transaction));

        protected abstract Task InsertAsync(TEntity entity, SqliteConnection connection, IDbTransaction transaction);
        protected abstract Task UpdateAsync(TEntity entity, SqliteConnection connection, IDbTransaction transaction);
        protected abstract Task DeleteAsync(TEntity entity, SqliteConnection connection, IDbTransaction transaction);

        protected async Task DeleteMovieSeriesEntryAsync(
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
