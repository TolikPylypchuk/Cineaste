using System;
using System.Collections.Generic;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("FranchiseEntries")]
    public sealed class FranchiseEntry : EntityBase
    {
        public int? MovieId { get; set; }

        [Write(false)]
        public Movie? Movie { get; set; }

        public int? SeriesId { get; set; }

        [Write(false)]
        public Series? Series { get; set; }

        public int? FranchiseId { get; set; }

        [Write(false)]
        public Franchise? Franchise { get; set; }

        public int ParentFranchiseId { get; set; }

        [Write(false)]
        public Franchise ParentFranchise { get; set; } = null!;

        public int SequenceNumber { get; set; }
        public int? DisplayNumber { get; set; }

        [Computed]
        public List<Title> Titles =>
            (this.Movie, this.Series, this.Franchise) switch
            {
                (var movie, null, null) when movie != null => movie.Titles,
                (null, var series, null) when series != null => series.Titles,
                (null, null, var franchise) when franchise != null => franchise.ActualTitles,
                _ => throw new InvalidOperationException("Exactly one franchise entry component must be non-null.")
            };

        public override string ToString() =>
            $"Franchise Entry #{this.Id}: {Title.ToString(this.Titles)}";
    }
}
