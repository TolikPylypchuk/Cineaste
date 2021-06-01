using MovieList.Core.ListItems;

using ReactiveUI;

namespace MovieList.Core.ViewModels
{
    public sealed class ListItemViewModel : ReactiveObject
    {
        public ListItemViewModel(ListItem item) =>
            this.Item = item;

        public ListItem Item { get; }

        public override string ToString() =>
            $"Item: {this.Item}";
    }
}
