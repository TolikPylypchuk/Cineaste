using System;
using System.Collections.Generic;

using Dapper.Contrib.Extensions;

using static Cineaste.Data.Constants;

namespace Cineaste.Data.Models
{
    [Table("Tags")]
    public sealed class Tag : EntityBase
    {
        public string Name { get; set; } = String.Empty;
        public string Category { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string Color { get; set; } = DefaultNewTagColor;

        public bool IsApplicableToMovies { get; set; } = true;
        public bool IsApplicableToSeries { get; set; } = true;

        [Write(false)]
        public HashSet<Movie> Movies { get; set; } = new();

        [Write(false)]
        public HashSet<Series> Series { get; set; } = new();

        [Write(false)]
        public HashSet<Tag> ImpliedTags { get; set; } = new();

        [Write(false)]
        public HashSet<Tag> InferredTags { get; set; } = new();

        public override string ToString() =>
            $"Tag #{this.Id}: {this.Name} ({this.Category})";
    }
}
