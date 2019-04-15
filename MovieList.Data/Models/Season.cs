using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Seasons))]
    public class Season : EntityBase
    {
        public bool IsWatched { get; set; }
        public bool IsReleased { get; set; }

        [Required]
        [StringLength(64)]
        public string Channel { get; set; } = String.Empty;

        [Url]
        [StringLength(256)]
        public string? ImdbLink { get; set; }

        [Url]
        [StringLength(256)]
        public string? PosterUrl { get; set; }

        public int SeriesId { get; set; }

        [ForeignKey(nameof(SeriesId))]
        public virtual Series Series { get; set; }

        public virtual IList<Title> Titles { get; set; } = new List<Title>();

        public virtual IList<Period> Periods { get; set; } = new List<Period>();

        [NotMapped]
        public Title Title
            => this.Titles
                .Where(title => !title.IsOriginal)
                .OrderByDescending(title => title.Priority)
                .First();

        [NotMapped]
        public Title OriginalTitle
            => this.Titles
                .Where(title => title.IsOriginal)
                .OrderByDescending(title => title.Priority)
                .First();

        public override string ToString()
            => $"Season: {this.Id}";
    }
}
