using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

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
            => this.WithTransactionAsync((connection, transaction) =>
                this.DeleteAsync(entity, connection, transaction));

        protected abstract Task InsertAsync(TEntity entity, DbConnection connection, IDbTransaction transaction);
        protected abstract Task UpdateAsync(TEntity entity, DbConnection connection, IDbTransaction transaction);
        protected abstract Task DeleteAsync(TEntity entity, DbConnection connection, IDbTransaction transaction);

        protected async Task DeleteMovieSeriesEntryAsync(
            MovieSeriesEntry movieSeriesEntry,
            DbConnection connection,
            IDbTransaction transaction)
        {
            await connection.DeleteAsync(movieSeriesEntry, transaction);

            var parentSeries = movieSeriesEntry.ParentSeries;

            foreach (var entry in parentSeries.Entries
                .Where(entry => entry.SequenceNumber > movieSeriesEntry.SequenceNumber))
            {
                entry.SequenceNumber--;

                if (entry.DisplayNumber != null)
                {
                    entry.DisplayNumber--;
                }

                await connection.UpdateAsync(entry, transaction);
            }

            parentSeries.Entries.Remove(movieSeriesEntry);

            if (parentSeries.Entries.Count == 0)
            {
                await connection.DeleteAsync(parentSeries, transaction);

                if (parentSeries.Entry != null)
                {
                    await this.DeleteMovieSeriesEntryAsync(parentSeries.Entry, connection, transaction);
                }
            } else
            {
                this.UpdateMergedDisplayNumbers(movieSeriesEntry.MovieSeries ?? parentSeries);
            }
        }

        protected void UpdateMergedDisplayNumbers(MovieSeries movieSeries)
        {
            if (movieSeries.Entry == null)
            {
                return;
            }

            int maxDisplayNumber = movieSeries.Entries.Select(entry => entry.DisplayNumber).Max() ?? 0;

            movieSeries.Entry.ParentSeries.Entries
                .OrderBy(entry => entry.SequenceNumber)
                .SkipWhile(entry => entry.SequenceNumber <= movieSeries.Entry.SequenceNumber)
                .TakeWhile(entry => entry.MovieSeries != null && entry.MovieSeries.MergeDisplayNumbers)
                .Aggregate(maxDisplayNumber + 1, this.UpdateMergedDisplayNumbers);
        }

        private int UpdateMergedDisplayNumbers(int firstDisplayNumber, MovieSeriesEntry entry)
            => entry.MovieSeries!.Entries
                .OrderBy(e => e.SequenceNumber)
                .Aggregate(firstDisplayNumber, (num, e) =>
                {
                    if (e.DisplayNumber != null)
                    {
                        e.DisplayNumber = num;
                        return num + 1;
                    }

                    return num;
                });
    }
}
