using System;

using MovieList.ViewModels.ListItems;

namespace MovieList.Events
{
    public class ListItemUpdatedEventArgs : EventArgs
    {
        public ListItemUpdatedEventArgs(ListItem item)
            => this.Item = item;

        public ListItem Item { get; }
    }
}
