using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using MovieList.Data.Properties;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Movies))]
    public class Movie : EntityBase
    {
        [Range(1850, 2100, ErrorMessageResourceName = "Movie.InvalidYear", ErrorMessageResourceType = typeof(Messages))]
        public int Year { get; set; }

        public bool IsWatched { get; set; }
        public bool IsReleased { get; set; }

        [Url(ErrorMessageResourceName = "ImdbLinkInvalid", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(256, ErrorMessageResourceName = "ImdbLinkTooLong", ErrorMessageResourceType = typeof(Messages))]
        public string? ImdbLink { get; set; }

        [Url(ErrorMessageResourceName = "PosterUrlInvalid", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(256, ErrorMessageResourceName = "PosterUrlTooLong", ErrorMessageResourceType = typeof(Messages))]
        public string? PosterUrl { get; set; }

        public int KindId { get; set; }

        [ForeignKey(nameof(KindId))]
        public virtual Kind Kind { get; set; }

        public virtual MovieSeriesEntry? Entry { get; set; }

        public virtual IList<Title> Titles { get; set; } = new List<Title>();

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
            => $"Movie: {this.Id}";
    }
}
