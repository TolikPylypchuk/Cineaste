using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Kinds))]
    public class Kind : EntityBase
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; } = String.Empty;

        public string ColorForMovie { get; set; }
        public string ColorForSeries { get; set; }

        public virtual IList<Movie> Movies { get; set; } = new List<Movie>();

        public virtual IList<Series> Series { get; set; } = new List<Series>();

        public override string ToString()
            => $"Kind: {this.Id}";
    }
}
