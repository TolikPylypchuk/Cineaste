using System;
using System.Collections.Generic;
using System.Linq;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("SpecialEpisodes")]
    public sealed class SpecialEpisode : EntityBase
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public bool IsWatched { get; set; }
        public bool IsReleased { get; set; }

        public string Channel { get; set; } = String.Empty;

        public int SequenceNumber { get; set; }

        public string? PosterUrl { get; set; }

        public int SeriesId { get; set; }

        [Write(false)]
        public Series Series { get; set; } = null!;

        public IList<Title> Titles { get; set; } = new List<Title>();

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

        public override string ToString()
            => $"Special Episode #{this.Id}: {Title.ToString(this.Titles)}";
    }
}
