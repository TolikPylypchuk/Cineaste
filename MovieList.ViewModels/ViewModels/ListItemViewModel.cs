using MovieList.ListItems;

using ReactiveUI;

namespace MovieList.ViewModels
{
    [ToString]
    public sealed class ListItemViewModel : ReactiveObject
    {
        public ListItemViewModel(ListItem item)
            => this.Item = item;

        public ListItem Item { get; }
    }
}
