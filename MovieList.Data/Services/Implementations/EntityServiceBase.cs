using System.Data;
using System.Linq;

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

        public void Save(TEntity entity)
            => this.WithTransaction((connection, transaction) =>
            {
                if (entity.Id == default)
                {
                    this.Insert(entity, connection, transaction);
                } else
                {
                    this.Update(entity, connection, transaction);
                }
            });

        public void Delete(TEntity entity)
            => this.WithTransaction((connection, transaction) => this.Delete(entity, connection, transaction));

        protected abstract void Insert(TEntity entity, IDbConnection connection, IDbTransaction transaction);
        protected abstract void Update(TEntity entity, IDbConnection connection, IDbTransaction transaction);
        protected abstract void Delete(TEntity entity, IDbConnection connection, IDbTransaction transaction);

        protected void DeleteMovieSeriesEntry(
            MovieSeriesEntry movieSeriesEntry,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            connection.Delete(movieSeriesEntry, transaction);

            var parentSeries = movieSeriesEntry.ParentSeries;

            foreach (var entry in parentSeries.Entries
                .Where(entry => entry.SequenceNumber > movieSeriesEntry.SequenceNumber))
            {
                entry.SequenceNumber--;

                if (entry.DisplayNumber != null)
                {
                    entry.DisplayNumber--;
                }

                connection.Update(entry, transaction);
            }

            parentSeries.Entries.Remove(movieSeriesEntry);

            if (parentSeries.Entries.Count == 0)
            {
                connection.Delete(parentSeries, transaction);

                if (parentSeries.Entry != null)
                {
                    this.DeleteMovieSeriesEntry(parentSeries.Entry, connection, transaction);
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
