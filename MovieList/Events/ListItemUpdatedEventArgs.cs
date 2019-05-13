using MovieList.ViewModels.ListItems;

namespace MovieList.Events
{
    public class ListItemUpdatedEventArgs
    {
        public ListItemUpdatedEventArgs(ListItem item)
            => this.Item = item;

        public ListItem Item { get; }
    }
}
