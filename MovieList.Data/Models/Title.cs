using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Titles))]
    public class Title : EntityBase
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; } = String.Empty;

        [Range(1, 10)]
        public int Priority { get; set; }

        public bool IsOriginal { get; set; }

        public int? MovieId { get; set; }
        public int? SeriesId { get; set; }
        public int? MovieSeriesId { get; set; }
        public int? SeasonId { get; set; }
        public int? SpecialEpisodeId { get; set; }

        [ForeignKey(nameof(MovieId))]
        public virtual Movie? Movie { get; set; }

        [ForeignKey(nameof(SeriesId))]
        public virtual Series? Series { get; set; }

        [ForeignKey(nameof(MovieSeriesId))]
        public virtual MovieSeries? MovieSeries { get; set; }

        [ForeignKey(nameof(SeasonId))]
        public virtual Season? Season { get; set; }

        [ForeignKey(nameof(SpecialEpisodeId))]
        public virtual SpecialEpisode? SpecialEpisode { get; set; }

        public override string ToString()
            => $"Title: {this.Id}";
    }
}
