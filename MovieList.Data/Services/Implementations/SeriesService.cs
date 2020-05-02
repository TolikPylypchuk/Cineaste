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

        protected override async Task InsertAsync(Series series, SqliteConnection connection, IDbTransaction transaction)
        {
            series.KindId = series.Kind.Id;

            series.Id = await connection.InsertAsync(series, transaction);

            foreach (var title in series.Titles)
            {
                title.SeriesId = series.Id;
                title.Id = await connection.InsertAsync(title, transaction);
            }

            foreach (var season in series.Seasons)
            {
                season.SeriesId = series.Id;
                season.Id = await connection.InsertAsync(season, transaction);
                await this.InsertSeasonDependentEntities(season, connection, transaction);
            }

            foreach (var episode in series.SpecialEpisodes)
            {
                episode.SeriesId = series.Id;
                episode.Id = await connection.InsertAsync(episode, transaction);
                await this.InsertSpecialEpisodeDependentEntities(episode, connection, transaction);
            }

            if (series.Entry != null)
            {
                var entry = series.Entry;
                entry.SeriesId = series.Id;
                entry.ParentSeriesId = entry.ParentSeries.Id;
                entry.Id = await connection.InsertAsync(entry, transaction);
                entry.ParentSeries.Entries.Add(entry);

                this.UpdateMergedDisplayNumbers(entry.ParentSeries);
            }
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
                (season, seriesId) => season.SeriesId = seriesId,
                season => this.InsertSeasonDependentEntities(season, connection, transaction),
                season => this.DeleteSeasonDependentEntities(season, connection, transaction));

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
                (episode, seriesId) => episode.SeriesId = seriesId,
                episode => this.InsertSpecialEpisodeDependentEntities(episode, connection, transaction),
                episode => this.DeleteSpecialEpisodeDependentEntities(episode, connection, transaction));

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

        private async Task InsertSeasonDependentEntities(
            Season season,
            SqliteConnection connection,
            IDbTransaction transaction)
        {
            foreach (var title in season.Titles)
            {
                title.SeasonId = season.Id;
                title.Id = await connection.InsertAsync(title, transaction);
            }

            foreach (var period in season.Periods)
            {
                period.SeasonId = season.Id;
                period.Id = await connection.InsertAsync(period, transaction);
            }
        }

        private async Task DeleteSeasonDependentEntities(
            Season season,
            SqliteConnection connection,
            IDbTransaction transaction)
        {
            await connection.DeleteAsync(season.Periods, transaction);
            await connection.DeleteAsync(season.Titles, transaction);
        }

        private async Task InsertSpecialEpisodeDependentEntities(
            SpecialEpisode episode,
            SqliteConnection connection,
            IDbTransaction transaction)
        {
            foreach (var title in episode.Titles)
            {
                title.SpecialEpisodeId = episode.Id;
                title.Id = await connection.InsertAsync(title, transaction);
            }
        }

        private async Task DeleteSpecialEpisodeDependentEntities(
            SpecialEpisode episode,
            SqliteConnection connection,
            IDbTransaction transaction)
        {
            await connection.DeleteAsync(episode.Titles, transaction);
        }
    }
}
