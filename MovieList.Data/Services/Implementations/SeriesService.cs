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

        protected override async Task UpdateAsync(Series series, SqliteConnection connection, IDbTransaction transaction)
        {
            await connection.UpdateAsync(series, transaction);

            var updater = new DependentEntityUpdater(connection, transaction);

            await updater.UpdateDependentEntitiesAsync(
                series,
                series.Titles,
                title => title.SeriesId,
                (title, seriesId) => title.SeriesId = seriesId);

            await updater.UpdateDependentEntitiesAsync(
                series,
                series.Seasons,
                season => season.SeriesId,
                (season, seriesId) => season.SeriesId = seriesId);

            foreach (var season in series.Seasons)
            {
                await updater.UpdateDependentEntitiesAsync(
                    season,
                    season.Periods,
                    period => period.SeasonId,
                    (period, seasonId) => period.SeasonId = seasonId);

                await updater.UpdateDependentEntitiesAsync(
                    season,
                    season.Titles,
                    title => title.SeasonId,
                    (title, seasonId) => title.SeasonId = seasonId);
            }

            await updater.UpdateDependentEntitiesAsync(
                series,
                series.SpecialEpisodes,
                episode => episode.SeriesId,
                (episode, seriesId) => episode.SeriesId = seriesId);

            if (series.Entry != null)
            {
                await connection.UpdateAsync(series.Entry, transaction);
            }
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
