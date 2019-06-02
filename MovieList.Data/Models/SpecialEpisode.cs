using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.SpecialEpisodes))]
    public class SpecialEpisode : EntityBase
    {
        [Range(1, 12)]
        public int Month { get; set; }

        [Range(1950, 2100)]
        public int Year { get; set; }

        public bool IsWatched { get; set; }
        public bool IsReleased { get; set; }

        [Required]
        [StringLength(64)]
        public string Channel { get; set; } = String.Empty;

        public int OrdinalNumber { get; set; }

        [Url]
        [StringLength(256)]
        public string? PosterUrl { get; set; }

        public int SeriesId { get; set; }

        [ForeignKey(nameof(SeriesId))]
        public virtual Series Series { get; set; }

        public virtual IList<Title> Titles { get; set; } = new List<Title>();

        [NotMapped]
        public Title Title
            => this.Titles
                .Where(title => !title.IsOriginal)
                .OrderBy(title => title.Priority)
                .First();

        [NotMapped]
        public Title OriginalTitle
            => this.Titles
                .Where(title => title.IsOriginal)
                .OrderBy(title => title.Priority)
                .First();

        public override string ToString()
            => $"Special Episode: {this.Id}";
    }
}
