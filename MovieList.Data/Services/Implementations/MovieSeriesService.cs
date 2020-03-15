using System.Collections.Generic;
using System.Data;
using System.Linq;
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

            await this.InsertEntriesAsync(movieSeries.Entries, movieSeries.Id, connection, transaction);

            if (movieSeries.Entry != null)
            {
                var entry = movieSeries.Entry;
                entry.MovieSeriesId = movieSeries.Id;
                entry.ParentSeriesId = entry.ParentSeries.Id;
                entry.Id = await connection.InsertAsync(entry, transaction);
                entry.ParentSeries.Entries.Add(entry);

                this.UpdateMergedDisplayNumbers(movieSeries);
            }
        }

        protected override async Task UpdateAsync(
            MovieSeries movieSeries,
            SqliteConnection connection,
            IDbTransaction transaction)
        {
            await connection.UpdateAsync(movieSeries, transaction);

            if (movieSeries.Entry != null)
            {
                await connection.UpdateAsync(movieSeries.Entry, transaction);
                this.UpdateMergedDisplayNumbers(movieSeries);
            }

            await new DependentEntityUpdater(connection, transaction).UpdateDependentEntitiesAsync(
                movieSeries,
                movieSeries.Titles,
                title => title.MovieSeriesId,
                (title, movieSeriesId) => title.MovieSeriesId = movieSeriesId);

            await this.InsertEntriesAsync(
                movieSeries.Entries.Where(entry => entry.Id == default), movieSeries.Id, connection, transaction);

            await new DependentEntityUpdater(connection, transaction).UpdateDependentEntitiesAsync(
                movieSeries,
                movieSeries.Entries,
                entry => entry.ParentSeriesId,
                (entry, movieSeriesId) => entry.ParentSeriesId = movieSeriesId);
        }

        protected override async Task DeleteAsync(
            MovieSeries movieSeries,
            SqliteConnection connection,
            IDbTransaction transaction)
        {
            await connection.DeleteAsync(movieSeries.Titles, transaction);
            await connection.DeleteAsync(movieSeries.Entries, transaction);

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
                await this.DeleteMovieSeriesEntryAsync(movieSeries.Entry, connection, transaction);
            }

            await connection.DeleteAsync(movieSeries, transaction);
        }

        private async Task InsertEntriesAsync(
            IEnumerable<MovieSeriesEntry> entries,
            int movieSeriesId,
            SqliteConnection connection,
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

                entry.Id = await connection.InsertAsync(entry, transaction);

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
