using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using MovieList.Data.Properties;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.SpecialEpisodes))]
    public class SpecialEpisode : EntityBase
    {
        [Range(1, 12, ErrorMessageResourceName = "InvalidMonth", ErrorMessageResourceType = typeof(Messages))]
        public int Month { get; set; }

        [Range(1950, 2100, ErrorMessageResourceName = "TV.InvalidYear", ErrorMessageResourceType = typeof(Messages))]
        public int Year { get; set; }

        public bool IsWatched { get; set; }
        public bool IsReleased { get; set; }

        [Required(ErrorMessageResourceName = "Season.ChannelRequired", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(64, ErrorMessageResourceName = "Season.ChannelTooLong", ErrorMessageResourceType = typeof(Messages))]
        public string Channel { get; set; } = String.Empty;

        [Url(ErrorMessageResourceName = "ImdbLinkInvalid", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(256, ErrorMessageResourceName = "ImdbLinkTooLong", ErrorMessageResourceType = typeof(Messages))]
        public string? ImdbLink { get; set; }

        [Url(ErrorMessageResourceName = "Movie.PosterUrlInvalid", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(256, ErrorMessageResourceName = "Movie.PosterUrlTooLong", ErrorMessageResourceType = typeof(Messages))]
        public string? PosterUrl { get; set; }

        public int SeriesId { get; set; }

        [ForeignKey(nameof(SeriesId))]
        public virtual Series Series { get; set; }

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
    }
}
