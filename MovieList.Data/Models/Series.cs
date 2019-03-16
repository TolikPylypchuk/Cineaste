using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MovieList.Data.Properties;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Series))]
    public class Series : EntityBase
    {
        public bool IsWatched { get; set; }

        [Url(ErrorMessageResourceName = "ImdbLinkInvalid", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(256, ErrorMessageResourceName = "ImdbLinkTooLong", ErrorMessageResourceType = typeof(Messages))]
        public string? ImdbLink { get; set; }

        [Url(ErrorMessageResourceName = "Movie.PosterUrlInvalid", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(256, ErrorMessageResourceName = "Movie.PosterUrlTooLong", ErrorMessageResourceType = typeof(Messages))]
        public string? PosterUrl { get; set; }

        public int KindId { get; set; }

        [ForeignKey(nameof(KindId))]
        public virtual Kind Kind { get; set; }

        public virtual MovieSeriesEntry? Entry { get; set; }

        public virtual IList<Title> Titles { get; set; } = new List<Title>();

        public virtual IList<Season> Seasons { get; set; } = new List<Season>();

        public virtual IList<SpecialEpisode> SpecialEpisodes { get; set; } = new List<SpecialEpisode>();
    }
}

#pragma warning enable CS8618 // Non-nullable field is uninitialized.
