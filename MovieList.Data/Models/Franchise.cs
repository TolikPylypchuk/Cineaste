using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public IList<FranchiseEntry> Entries { get; set; } = new List<FranchiseEntry>();

        [Write(false)]
        public IList<Title> Titles { get; set; } = new List<Title>();

        [Computed]
        [SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        public IList<Title> ActualTitles
            => this.Titles.Count != 0
                ? this.Titles
                : this.Entries.OrderBy(e => e.SequenceNumber).FirstOrDefault()?.Titles ?? new List<Title>();

        [Computed]
        public Title? Title
            => this.Titles
                .Where(title => !title.IsOriginal)
                .OrderBy(title => title.Priority)
                .FirstOrDefault();

        [Computed]
        public Title? OriginalTitle
            => this.Titles
                .Where(title => title.IsOriginal)
                .OrderBy(title => title.Priority)
                .FirstOrDefault();

        public override string ToString()
            => $"Franchise #{this.Id}: {Title.ToString(this.ActualTitles)}";
    }
}
