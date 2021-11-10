namespace Cineaste.Core.ViewModels.Filters;

public sealed class ListFilterViewModel : FilterItemHolder
{
    public ListFilterViewModel(ReadOnlyObservableCollection<Kind> kinds, ReadOnlyObservableCollection<Tag> tags)
        : base(kinds, tags)
    {
        this.Apply = ReactiveCommand.Create(() => this.FilterItem.CreateFilter());
        this.Clear.InvokeCommand(this.Apply);
    }

    public ReactiveCommand<Unit, Func<ListItem, bool>> Apply { get; }
}
