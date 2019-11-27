using System;
using System.Collections.Generic;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Seasons")]
    public sealed class Season : EntityBase
    {
        public bool IsWatched { get; set; }
        public bool IsReleased { get; set; }

        public string Channel { get; set; } = String.Empty;

        public int SequenceNumber { get; set; }

        public int SeriesId { get; set; }

        [Write(false)]
        public Series Series { get; set; } = null!;

        public IList<Title> Titles { get; set; } = new List<Title>();

        public IList<Period> Periods { get; set; } = new List<Period>();

        public override string ToString()
            => $"Series #{this.Id}: {Title.ToString(this.Titles)} ({this.Channel})";
    }
}
