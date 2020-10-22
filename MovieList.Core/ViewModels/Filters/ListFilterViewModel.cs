using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using MovieList.Core.ListItems;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class ListFilterViewModel : ReactiveObject
    {
        private readonly ReadOnlyObservableCollection<Kind> kinds;
        private readonly ReadOnlyObservableCollection<Tag> tags;

        public ListFilterViewModel(ReadOnlyObservableCollection<Kind> kinds, ReadOnlyObservableCollection<Tag> tags)
        {
            this.kinds = kinds;
            this.tags = tags;

            this.ApplyFilter = ReactiveCommand.Create(() => this.FilterItem.CreateFilter());
            this.ClearFilter = ReactiveCommand.Create(() => { });

            this.ClearFilter
                .StartWith(Unit.Default)
                .Select(this.CreateDefaultFilterItem)
                .BindTo(this, vm => vm.FilterItem);

            this.ClearFilter.InvokeCommand(this.ApplyFilter);
        }

        [Reactive]
        public bool IsAvailable { get; set; } = true;

        [Reactive]
        public FilterItem FilterItem { get; set; } = null!;

        public ReactiveCommand<Unit, Func<ListItem, bool>> ApplyFilter { get; }
        public ReactiveCommand<Unit, Unit> ClearFilter { get; }

        private FilterItem CreateDefaultFilterItem()
        {
            var filterItem = new SimpleFilterItemViewModel(kinds, tags);

            var subscriptions = new CompositeDisposable();

            filterItem.Delete
                .InvokeCommand(this.ClearFilter)
                .DisposeWith(subscriptions);

            filterItem.MakeComposite
                .Select(composition => CompositeFilterItemViewModel.FromSimpleItem(filterItem, composition))
                .BindTo(this, vm => vm.FilterItem)
                .DisposeWith(subscriptions);

            filterItem.Delete
                .Merge(filterItem.MakeComposite.Discard())
                .Subscribe(subscriptions.Dispose)
                .DisposeWith(subscriptions);

            return filterItem;
        }
    }
}
