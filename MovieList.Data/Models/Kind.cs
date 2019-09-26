using System;
using System.Collections.Generic;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Kinds")]
    public sealed class Kind : EntityBase
    {
        public string Name { get; set; } = String.Empty;

        public string ColorForMovie { get; set; } = String.Empty;
        public string ColorForSeries { get; set; } = String.Empty;

        public IList<Movie> Movies { get; set; } = new List<Movie>();
        public IList<Series> Series { get; set; } = new List<Series>();

        public override string ToString()
            => $"Kind #{this.Id}: {this.Name}";
    }
}
