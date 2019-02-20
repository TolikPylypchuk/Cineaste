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

        public int ColorForMovie { get; set; }
        public int ColorForSeries { get; set; }

        public virtual List<Movie> Movies { get; set; } = new List<Movie>();

        public virtual List<Series> Series { get; set; } = new List<Series>();
    }
}
