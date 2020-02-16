using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("MovieSeries")]
    public sealed class MovieSeries : EntityBase
    {
        public bool ShowTitles { get; set; }
        public bool IsLooselyConnected { get; set; }
        public bool MergeDisplayNumbers { get; set; }

        public string? PosterUrl { get; set; }

        [Write(false)]
        public MovieSeriesEntry? Entry { get; set; }

        [Write(false)]
        public IList<MovieSeriesEntry> Entries { get; set; } = new List<MovieSeriesEntry>();

        [Write(false)]
        public IList<Title> Titles { get; set; } = new List<Title>();

        [Computed]
        [SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        public IList<Title> ActualTitles
            => this.Titles.Count != 0
                ? this.Titles
                : this.Entries.OrderBy(e => e.SequenceNumber).FirstOrDefault()?.Titles ?? new List<Title>();

        [Computed]
        public Title? Title
            => this.Titles
                .Where(title => !title.IsOriginal)
                .OrderBy(title => title.Priority)
                .FirstOrDefault();

        [Computed]
        public Title? OriginalTitle
            => this.Titles
                .Where(title => title.IsOriginal)
                .OrderBy(title => title.Priority)
                .FirstOrDefault();

        public override string ToString()
            => $"Movie Series #{this.Id}: {Title.ToString(this.ActualTitles)}";
    }
}
