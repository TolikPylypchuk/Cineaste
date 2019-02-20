using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

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

        [Url]
        [StringLength(256)]
        public string? ImdbLink { get; set; }

        [Url]
        [StringLength(256)]
        public string? PosterUrl { get; set; }

        public int SeriesId { get; set; }

        [ForeignKey(nameof(SeriesId))]
        public virtual Series Series { get; set; }

        public virtual List<Title> Titles { get; set; } = new List<Title>();
    }
}

#pragma warning enable CS8618 // Non-nullable field is uninitialized.
