using System.ComponentModel.DataAnnotations.Schema;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.MovieSeriesEntries))]
    public class MovieSeriesEntry : EntityBase
    {
        public int? MovieId { get; set; }
        public int? SeriesId { get; set; }
        public int MovieSeriesId { get; set; }

        public int OrdinalNumber { get; set; }

        public int? DisplayNumber { get; set; }

        [ForeignKey(nameof(MovieId))]
        public virtual Movie? Movie { get; set; }

        [ForeignKey(nameof(MovieId))]
        public virtual Series? Series { get; set; }

        [ForeignKey(nameof(MovieSeriesId))]
        public virtual MovieSeries MovieSeries { get; set; }
    }
}
