﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MovieList.Data.Properties;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Seasons))]
    public class Season : EntityBase
    {
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

        public virtual IList<Period> Periods { get; set; } = new List<Period>();
    }
}

#pragma warning enable CS8618 // Non-nullable field is uninitialized.
