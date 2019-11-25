using MovieList.ListItems;

using ReactiveUI;

namespace MovieList.ViewModels
{
    public sealed class ListItemViewModel : ReactiveObject
    {
        public ListItemViewModel(ListItem item)
            => this.Item = item;

        public ListItem Item { get; }

        public override string ToString()
            => $"Item: {this.Item}";
    }
}
