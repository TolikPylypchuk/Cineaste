using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class SeriesService : TaggedEntityServiceBase<Series, SeriesTag>
    {
        public SeriesService(string fileName)
            : base(fileName, CompositeIdEqualityComparer.SeriesTag)
        { }

        protected override void Insert(Series series, IDbConnection connection, IDbTransaction transaction)
        {
            series.KindId = series.Kind.Id;

            series.Id = (int)connection.Insert(series, transaction);

            foreach (var title in series.Titles)
            {
                title.SeriesId = series.Id;
                title.Id = (int)connection.Insert(title, transaction);
            }

            foreach (var season in series.Seasons)
            {
                season.SeriesId = series.Id;
                season.Id = (int)connection.Insert(season, transaction);
                this.InsertSeasonDependentEntities(season, connection, transaction);
            }

            foreach (var episode in series.SpecialEpisodes)
            {
                episode.SeriesId = series.Id;
                episode.Id = (int)connection.Insert(episode, transaction);
                this.InsertSpecialEpisodeDependentEntities(episode, connection, transaction);
            }

            if (series.Entry != null)
            {
                var entry = series.Entry;
                entry.SeriesId = series.Id;
                entry.ParentFranchiseId = entry.ParentFranchise.Id;
                entry.Id = (int)connection.Insert(entry, transaction);
                entry.ParentFranchise.Entries.Add(entry);

                this.UpdateMergedDisplayNumbers(entry.ParentFranchise);
            }
        }

        protected override void Update(Series series, IDbConnection connection, IDbTransaction transaction)
        {
            series.KindId = series.Kind.Id;

            connection.Update(series, transaction);

            var updater = new DependentEntityUpdater(connection, transaction);

            updater.UpdateDependentEntities(
                series,
                series.Titles,
                title => title.SeriesId,
                (title, seriesId) => title.SeriesId = seriesId);

            updater.UpdateDependentEntities(
                series,
                series.Seasons,
                season => season.SeriesId,
                (season, seriesId) => season.SeriesId = seriesId,
                season => this.InsertSeasonDependentEntities(season, connection, transaction),
                season => this.DeleteSeasonDependentEntities(season, connection, transaction));

            foreach (var season in series.Seasons)
            {
                updater.UpdateDependentEntities(
                    season,
                    season.Periods,
                    period => period.SeasonId,
                    (period, seasonId) => period.SeasonId = seasonId);

                updater.UpdateDependentEntities(
                    season,
                    season.Titles,
                    title => title.SeasonId,
                    (title, seasonId) => title.SeasonId = seasonId);
            }

            updater.UpdateDependentEntities(
                series,
                series.SpecialEpisodes,
                episode => episode.SeriesId,
                (episode, seriesId) => episode.SeriesId = seriesId,
                episode => this.InsertSpecialEpisodeDependentEntities(episode, connection, transaction),
                episode => this.DeleteSpecialEpisodeDependentEntities(episode, connection, transaction));

            if (series.Entry != null)
            {
                connection.Update(series.Entry, transaction);
            }
        }

        protected override void Delete(Series series, IDbConnection connection, IDbTransaction transaction)
        {
            connection.Delete(series.Titles, transaction);

            connection.Delete(series.Seasons.SelectMany(season => season.Periods), transaction);
            connection.Delete(series.Seasons.SelectMany(season => season.Titles), transaction);
            connection.Delete(series.Seasons, transaction);

            connection.Delete(series.SpecialEpisodes.SelectMany(episode => episode.Titles), transaction);
            connection.Delete(series.SpecialEpisodes, transaction);

            if (series.Entry != null)
            {
                this.DeleteFranchiseEntry(series.Entry, connection, transaction);
            }

            connection.Delete(series, transaction);
        }

        protected override List<SeriesTag> GetTags(Series series) =>
            series.Tags
                .Select(tag => new SeriesTag { SeriesId = series.Id, TagId = tag.Id })
                .ToList();

        private void InsertSeasonDependentEntities(Season season, IDbConnection connection, IDbTransaction transaction)
        {
            foreach (var title in season.Titles)
            {
                title.SeasonId = season.Id;
                title.Id = (int)connection.Insert(title, transaction);
            }

            foreach (var period in season.Periods)
            {
                period.SeasonId = season.Id;
                period.Id = (int)connection.Insert(period, transaction);
            }
        }

        private void DeleteSeasonDependentEntities(Season season, IDbConnection connection, IDbTransaction transaction)
        {
            connection.Delete(season.Periods, transaction);
            connection.Delete(season.Titles, transaction);
        }

        private void InsertSpecialEpisodeDependentEntities(
            SpecialEpisode episode,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            foreach (var title in episode.Titles)
            {
                title.SpecialEpisodeId = episode.Id;
                title.Id = (int)connection.Insert(title, transaction);
            }
        }

        private void DeleteSpecialEpisodeDependentEntities(
            SpecialEpisode episode,
            IDbConnection connection,
            IDbTransaction transaction) =>
            connection.Delete(episode.Titles, transaction);
    }
}
