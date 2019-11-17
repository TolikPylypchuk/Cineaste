using System;
using System.Collections.Generic;
using System.Linq;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Series")]
    public sealed class Series : EntityBase
    {
        public bool IsWatched { get; set; }
        public bool IsMiniseries { get; set; }
        public bool IsAnthology { get; set; }

        public SeriesStatus Status { get; set; } = SeriesStatus.NotStarted;

        public string? ImdbLink { get; set; }
        public string? PosterUrl { get; set; }

        public int KindId { get; set; }
        public Kind Kind { get; set; } = null!;

        public MovieSeriesEntry? Entry { get; set; }

        public IList<Title> Titles { get; set; } = new List<Title>();

        public IList<Season> Seasons { get; set; } = new List<Season>();

        public IList<SpecialEpisode> SpecialEpisodes { get; set; } = new List<SpecialEpisode>();

        [Computed]
        public Title Title
            => this.Titles
                .Where(title => !title.IsOriginal)
                .OrderBy(title => title.Priority)
                .First();

        [Computed]
        public Title OriginalTitle
            => this.Titles
                .Where(title => title.IsOriginal)
                .OrderBy(title => title.Priority)
                .First();

        [Computed]
        public int StartYear
            => Math.Min(
                this.Seasons
                    .SelectMany(season => season.Periods)
                    .OrderBy(period => period.StartYear)
                    .ThenBy(period => period.StartMonth)
                    .First()
                    .StartYear,
                this.SpecialEpisodes
                    .OrderBy(episode => episode.Year)
                    .ThenBy(episode => episode.Month)
                    .FirstOrDefault()
                    ?.Year
                ?? Int32.MaxValue);

        [Computed]
        public int EndYear
            => Math.Max(
                this.Seasons
                    .SelectMany(season => season.Periods)
                    .OrderByDescending(period => period.EndYear)
                    .ThenByDescending(period => period.EndMonth)
                    .First()
                    .EndYear,
                this.SpecialEpisodes
                    .OrderByDescending(episode => episode.Year)
                    .ThenByDescending(episode => episode.Month)
                    .FirstOrDefault()
                    ?.Year
                ?? Int32.MinValue);

        public override string ToString()
            => $"Series #{this.Id}: {Title.ToString(this.Titles)}";
    }
}
