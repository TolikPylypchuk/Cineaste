using System;
using System.Collections.Generic;

using Dapper.Contrib.Extensions;

using static MovieList.Data.Constants;

namespace MovieList.Data.Models
{
    [Table("Kinds")]
    public sealed class Kind : EntityBase
    {
        public string Name { get; set; } = String.Empty;

        public string ColorForWatchedMovie { get; set; } = DefaultNewKindColor;
        public string ColorForWatchedSeries { get; set; } = DefaultNewKindColor;

        public string ColorForNotWatchedMovie { get; set; } = DefaultNewKindColor;
        public string ColorForNotWatchedSeries { get; set; } = DefaultNewKindColor;

        public string ColorForNotReleasedMovie { get; set; } = DefaultNewKindColor;
        public string ColorForNotReleasedSeries { get; set; } = DefaultNewKindColor;

        [Write(false)]
        public List<Movie> Movies { get; set; } = new();

        [Write(false)]
        public List<Series> Series { get; set; } = new();

        public override string ToString() =>
            $"Kind #{this.Id}: {this.Name}";
    }
}
