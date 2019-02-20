using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.MovieSeriesEntries))]
    public class MovieSeriesEntry : EntityBase
    {
        public int? MovieId { get; set; }
        public int? SeriesId { get; set; }
        public int MovieSeriesId { get; set; }

        public int OrdinalNumber { get; set; }

        public bool ShowOrdinalNumber { get; set; }

        [ForeignKey(nameof(MovieId))]
        public virtual Movie? Movie { get; set; }

        [ForeignKey(nameof(MovieId))]
        public virtual Series? Series { get; set; }

        [ForeignKey(nameof(MovieSeriesId))]
        public virtual MovieSeries MovieSeries { get; set; }
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized.
