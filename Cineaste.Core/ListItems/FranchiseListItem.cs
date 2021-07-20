using System;

using Cineaste.Core.Data.Models;
using Cineaste.Data.Models;

namespace Cineaste.Core.ListItems
{
    public class FranchiseListItem : ListItem
    {
        public FranchiseListItem(Franchise franchise)
            : base(
                $"F-{franchise.Id}",
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
                franchise.GetActiveColor()) =>
            this.Franchise = franchise;

        public Franchise Franchise { get; }

        public override FranchiseEntry? Entry =>
            this.Franchise.Entry;
    }
}
