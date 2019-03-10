using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Series))]
    public class Series : EntityBase
    {
        public bool IsWatched { get; set; }

        [Url]
        [StringLength(256)]
        public string? ImdbLink { get; set; }

        [Url]
        [StringLength(256)]
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
