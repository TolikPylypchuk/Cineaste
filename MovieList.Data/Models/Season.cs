using System;
using System.Collections.Generic;
using System.Linq;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Seasons")]
    public sealed class Season : EntityBase
    {
        public SeasonWatchStatus WatchStatus { get; set; } = SeasonWatchStatus.NotWatched;
        public SeasonReleaseStatus ReleaseStatus { get; set; } = SeasonReleaseStatus.NotStarted;

        public string Channel { get; set; } = String.Empty;

        public int SequenceNumber { get; set; }

        public int SeriesId { get; set; }

        [Write(false)]
        public Series Series { get; set; } = null!;

        [Write(false)]
        public IList<Title> Titles { get; set; } = new List<Title>();

        [Write(false)]
        public IList<Period> Periods { get; set; } = new List<Period>();

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
            => this.Periods
                .OrderBy(period => period.StartYear)
                .ThenBy(period => period.StartMonth)
                .First()
                .StartYear;

        [Computed]
        public int EndYear
            => this.Periods
                .OrderByDescending(period => period.EndYear)
                .ThenByDescending(period => period.EndMonth)
                .First()
                .EndYear;

        public override string ToString()
            => $"Series #{this.Id}: {Title.ToString(this.Titles)} ({this.Channel})";
    }
}
