using System;
using System.Collections.Generic;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Kinds")]
    public sealed class Kind : EntityBase
    {
        public string Name { get; set; } = String.Empty;

        public string ColorForWatchedMovie { get; set; } = String.Empty;
        public string ColorForWatchedSeries { get; set; } = String.Empty;

        public string ColorForNotWatchedMovie { get; set; } = String.Empty;
        public string ColorForNotWatchedSeries { get; set; } = String.Empty;

        public string ColorForNotReleasedMovie { get; set; } = String.Empty;
        public string ColorForNotReleasedSeries { get; set; } = String.Empty;

        [Write(false)]
        public IList<Movie> Movies { get; set; } = new List<Movie>();

        [Write(false)]
        public IList<Series> Series { get; set; } = new List<Series>();

        public override string ToString()
            => $"Kind #{this.Id}: {this.Name}";
    }
}
