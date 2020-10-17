using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

using MovieList.Core.ListItems;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class ListFilterViewModel : ReactiveObject
    {
        public ListFilterViewModel(ReadOnlyObservableCollection<Kind> kinds, ReadOnlyObservableCollection<Tag> tags)
        {
            this.ApplyFilter = ReactiveCommand.Create(() => this.FilterItem.CreateFilter());
            this.ClearFilter = ReactiveCommand.Create(() => { });

            this.ClearFilter
                .StartWith(Unit.Default)
                .Select(() => new FilterItemViewModel(kinds, tags))
                .ToPropertyEx(this, vm => vm.FilterItem);

            this.ClearFilter.InvokeCommand(this.ApplyFilter);
        }

        [Reactive]
        public bool IsAvailable { get; set; } = true;

        public FilterItemViewModel FilterItem { [ObservableAsProperty] get; } = null!;

        public ReactiveCommand<Unit, Func<ListItem, bool>> ApplyFilter { get; }
        public ReactiveCommand<Unit, Unit> ClearFilter { get; }
    }
}
