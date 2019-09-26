using System.Collections.Generic;
using System.Linq;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("MovieSeries")]
    public sealed class MovieSeries : EntityBase
    {
        public bool IsLooselyConnected { get; set; }
        public bool ShowTitles { get; set; }

        public string? PosterUrl { get; set; }

        public MovieSeriesEntry? Entry { get; set; }

        public IList<MovieSeriesEntry> Entries { get; set; } = new List<MovieSeriesEntry>();

        public IList<Title> Titles { get; set; } = new List<Title>();

        public IList<Title> ActualTitles
            => this.Titles.Count != 0
                ? this.Titles
                : this.Entries.OrderBy(e => e.SequenceNumber).First().Titles;

        public Title? Title
            => this.Titles
                .Where(title => !title.IsOriginal)
                .OrderBy(title => title.Priority)
                .FirstOrDefault();

        public Title? OriginalTitle
            => this.Titles
                .Where(title => title.IsOriginal)
                .OrderBy(title => title.Priority)
                .FirstOrDefault();

        public override string ToString()
            => $"Movie Series #{this.Id}: {Title.ToString(this.ActualTitles)}";
    }
}
