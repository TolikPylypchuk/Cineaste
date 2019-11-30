using System.Collections.Generic;

using MovieList.ListItems;
using MovieList.ViewModels;

namespace MovieList.Comparers
{
    public class ListItemViewModelComparer : NullsFirstComparer<ListItemViewModel>
    {
        private readonly IComparer<ListItem> itemComparer;

        public ListItemViewModelComparer(IComparer<ListItem> itemComparer)
            => this.itemComparer = itemComparer;

        protected override int CompareNonNull(ListItemViewModel x, ListItemViewModel y)
            => this.itemComparer.Compare(x.Item, y.Item);
    }
}
