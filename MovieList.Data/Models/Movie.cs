using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Movies))]
    public class Movie : EntityBase
    {
        [Range(1850, 2100)]
        public int Year { get; set; }

        public bool IsWatched { get; set; }
        public bool IsReleased { get; set; }

        [Url]
        [StringLength(256)]
        public string? ImdbLink { get; set; }

        [Url]
        [StringLength(256)]
        public string? PosterUrl { get; set; }

        public int KindId { get; set; }

        [ForeignKey(nameof(KindId))]
        public Kind Kind { get; set; }

        public virtual MovieSeriesEntry? Entry { get; set; }

        public virtual List<Title> Titles { get; set; } = new List<Title>();
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized.
