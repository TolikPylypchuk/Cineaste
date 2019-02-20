using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.MovieSeries))]
    public class MovieSeries : EntityBase
    {
        public bool IsLooselyConnected { get; set; }

        public int? ParentSeriesId { get; set; }

        public int? OrdinalNumber { get; set; }

        [ForeignKey(nameof(ParentSeriesId))]
        public MovieSeries? ParentSeries { get; set; }

        public virtual List<MovieSeriesEntry> Entries { get; set; } = new List<MovieSeriesEntry>();

        public virtual List<MovieSeries> Parts { get; set; } = new List<MovieSeries>();

        public virtual List<Title> Titles { get; set; } = new List<Title>();
    }
}
