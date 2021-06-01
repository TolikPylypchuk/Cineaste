using System.Collections.Generic;
using System.Linq;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Franchises")]
    public sealed class Franchise : EntityBase
    {
        public bool ShowTitles { get; set; }
        public bool IsLooselyConnected { get; set; }
        public bool MergeDisplayNumbers { get; set; }

        public string? PosterUrl { get; set; }

        [Write(false)]
        public FranchiseEntry? Entry { get; set; }

        [Write(false)]
        public List<FranchiseEntry> Entries { get; set; } = new();

        [Write(false)]
        public List<Title> Titles { get; set; } = new();

        [Computed]
        public List<Title> ActualTitles =>
            this.Titles.Count != 0
                ? this.Titles
                : this.Entries.OrderBy(e => e.SequenceNumber).FirstOrDefault()?.Titles ?? new();

        [Computed]
        public Title? Title =>
            this.Titles
                .Where(title => !title.IsOriginal)
                .OrderBy(title => title.Priority)
                .FirstOrDefault();

        [Computed]
        public Title? OriginalTitle =>
            this.Titles
                .Where(title => title.IsOriginal)
                .OrderBy(title => title.Priority)
                .FirstOrDefault();

        public override string ToString() =>
            $"Franchise #{this.Id}: {Title.ToString(this.ActualTitles)}";
    }
}
