using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

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

        public virtual List<Title> Titles { get; set; } = new List<Title>();

        public virtual List<Period> Periods { get; set; } = new List<Period>();
    }
}

#pragma warning enable CS8618 // Non-nullable field is uninitialized.
