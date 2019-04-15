using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Periods))]
    public class Period : EntityBase
    {
        [Range(1, 12)]
        public int StartMonth { get; set; }

        [Range(1950, 2100)]
        public int StartYear { get; set; }

        [Range(1, 12)]
        public int EndMonth { get; set; }

        [Range(1950, 2100)]
        public int EndYear { get; set; }

        public bool IsSingleDayRelease { get; set; }

        [Range(1, 50)]
        public int NumberOfEpisodes { get; set; }

        public int SeasonId { get; set; }

        [ForeignKey(nameof(SeasonId))]
        public virtual Season Season { get; set; }

        public override string ToString()
            => $"Period: {this.Id}";
    }
}
