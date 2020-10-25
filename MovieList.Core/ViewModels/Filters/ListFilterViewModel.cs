using System;
using System.Collections.ObjectModel;
using System.Reactive;

using MovieList.Core.ListItems;
using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.Core.ViewModels.Filters
{
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
}
