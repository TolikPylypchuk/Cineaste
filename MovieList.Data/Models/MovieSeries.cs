using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.MovieSeries))]
    public class MovieSeries : EntityBase
    {
        public bool IsLooselyConnected { get; set; }

        public int? ParentSeriesId { get; set; }

        public int? OrdinalNumber { get; set; }

        [ForeignKey(nameof(ParentSeriesId))]
        public virtual MovieSeries? ParentSeries { get; set; }

        public virtual IList<MovieSeriesEntry> Entries { get; set; } = new List<MovieSeriesEntry>();

        public virtual IList<MovieSeries> Parts { get; set; } = new List<MovieSeries>();

        public virtual IList<Title> Titles { get; set; } = new List<Title>();

        [NotMapped]
        public Title? Title
            => this.Titles
                .Where(title => !title.IsOriginal)
                .OrderBy(title => title.Priority)
                .FirstOrDefault();

        [NotMapped]
        public Title? OriginalTitle
            => this.Titles
                .Where(title => title.IsOriginal)
                .OrderBy(title => title.Priority)
                .FirstOrDefault();

        [NotMapped]
        public MovieSeries RootSeries
            => this.ParentSeries == null ? this : this.ParentSeries.RootSeries;

        public override string ToString()
            => $"Movie Series: {this.Id}";
    }
}
