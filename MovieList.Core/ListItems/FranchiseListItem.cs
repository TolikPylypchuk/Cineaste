using System;

using MovieList.Core.Data.Models;
using MovieList.Data.Models;

namespace MovieList.Core.ListItems
{
    public class FranchiseListItem : ListItem
    {
        public FranchiseListItem(Franchise franchise)
            : base(
                $"MS-{franchise.Id}",
                null,
                franchise.ShowTitles && franchise.Title != null
                    ? $"{franchise.Title.Name}:"
                    : String.Empty,
                franchise.ShowTitles && franchise.OriginalTitle != null
                    ? $"{franchise.OriginalTitle.Name}:"
                    : String.Empty,
                String.Empty,
                franchise.GetStartYear(),
                franchise.GetEndYear(),
                franchise.GetActiveColor())
            => this.Franchise = franchise;

        public Franchise Franchise { get; }

        public override FranchiseEntry? Entry
            => this.Franchise.Entry;
    }
}
