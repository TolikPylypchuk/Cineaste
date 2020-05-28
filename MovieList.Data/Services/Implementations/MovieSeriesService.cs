using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal class MovieSeriesService : EntityServiceBase<MovieSeries>
    {
        public MovieSeriesService(string file)
            : base(file)
        { }

        protected override void Insert(MovieSeries movieSeries, IDbConnection connection, IDbTransaction transaction)
        {
            movieSeries.Id = (int)connection.Insert(movieSeries, transaction);

            foreach (var title in movieSeries.Titles)
            {
                title.MovieSeriesId = movieSeries.Id;
                title.Id = (int)connection.Insert(title, transaction);
            }

            this.InsertEntries(movieSeries.Entries, movieSeries.Id, connection, transaction);

            if (movieSeries.Entry != null)
            {
                var entry = movieSeries.Entry;
                entry.MovieSeriesId = movieSeries.Id;
                entry.ParentSeriesId = entry.ParentSeries.Id;
                entry.Id = (int)connection.Insert(entry, transaction);
                entry.ParentSeries.Entries.Add(entry);

                this.UpdateMergedDisplayNumbers(movieSeries);
            }
        }

        protected override void Update(MovieSeries movieSeries, IDbConnection connection, IDbTransaction transaction)
        {
            connection.Update(movieSeries, transaction);

            if (movieSeries.Entry != null)
            {
                connection.Update(movieSeries.Entry, transaction);
                this.UpdateMergedDisplayNumbers(movieSeries);
            }

            new DependentEntityUpdater(connection, transaction).UpdateDependentEntities(
                movieSeries,
                movieSeries.Titles,
                title => title.MovieSeriesId,
                (title, movieSeriesId) => title.MovieSeriesId = movieSeriesId);

            this.InsertEntries(
                movieSeries.Entries.Where(entry => entry.Id == default), movieSeries.Id, connection, transaction);

            new DependentEntityUpdater(connection, transaction).UpdateDependentEntities(
                movieSeries,
                movieSeries.Entries,
                entry => entry.ParentSeriesId,
                (entry, movieSeriesId) => entry.ParentSeriesId = movieSeriesId);
        }

        protected override void Delete(MovieSeries movieSeries, IDbConnection connection, IDbTransaction transaction)
        {
            connection.DeleteAsync(movieSeries.Titles, transaction);
            connection.DeleteAsync(movieSeries.Entries, transaction);

            foreach (var entry in movieSeries.Entries)
            {
                if (entry.Movie != null)
                {
                    entry.Movie.Entry = null;
                } else if (entry.Series != null)
                {
                    entry.Series.Entry = null;
                } else if (entry.MovieSeries != null)
                {
                    entry.MovieSeries.Entry = null;
                }
            }

            if (movieSeries.Entry != null)
            {
                this.DeleteMovieSeriesEntry(movieSeries.Entry, connection, transaction);
            }

            connection.Delete(movieSeries, transaction);
        }

        private void InsertEntries(
            IEnumerable<MovieSeriesEntry> entries,
            int movieSeriesId,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            foreach (var entry in entries)
            {
                entry.ParentSeriesId = movieSeriesId;

                if (entry.Movie != null)
                {
                    entry.MovieId = entry.Movie.Id;
                } else if (entry.Series != null)
                {
                    entry.SeriesId = entry.Series.Id;
                } else if (entry.MovieSeries != null)
                {
                    entry.MovieSeriesId = entry.MovieSeries.Id;
                }

                entry.Id = (int)connection.Insert(entry, transaction);

                if (entry.Movie != null)
                {
                    entry.Movie.Entry = entry;
                } else if (entry.Series != null)
                {
                    entry.Series.Entry = entry;
                } else if (entry.MovieSeries != null)
                {
                    entry.MovieSeries.Entry = entry;
                }
            }
        }
    }
}
