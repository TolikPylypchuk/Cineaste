using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using Microsoft.Data.Sqlite;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class SeriesService : EntityServiceBase<Series>
    {
        public SeriesService(string fileName)
            : base(fileName)
        { }

        protected override Task InsertAsync(Series series, SqliteConnection connection, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        protected override Task UpdateAsync(Series series, SqliteConnection connection, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        protected override async Task DeleteAsync(Series series, SqliteConnection connection, IDbTransaction transaction)
        {
            await connection.DeleteAsync(series.Titles, transaction);

            await connection.DeleteAsync(series.Seasons.SelectMany(season => season.Periods), transaction);
            await connection.DeleteAsync(series.Seasons.SelectMany(season => season.Titles), transaction);
            await connection.DeleteAsync(series.Seasons, transaction);

            await connection.DeleteAsync(series.SpecialEpisodes.SelectMany(episode => episode.Titles), transaction);
            await connection.DeleteAsync(series.SpecialEpisodes, transaction);

            if (series.Entry != null)
            {
                await this.DeleteMovieSeriesEntryAsync(series.Entry, connection, transaction);
            }

            await connection.DeleteAsync(series, transaction);
        }
    }
}
