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
        public ListFilterViewModel(ReadOnlyObservableCollection<Kind> kinds, ReadOnlyObservableCollection<Tag> tags)
        {
            this.ApplyFilter = ReactiveCommand.Create(() => this.FilterItem.CreateFilter());
            this.ClearFilter = ReactiveCommand.Create(() => { });

            this.ClearFilter
                .StartWith(Unit.Default)
                .Select(() => this.WithSubscriptions(new SimpleFilterItemViewModel(kinds, tags)))
                .BindTo(this, vm => vm.FilterItem);

            this.ClearFilter.InvokeCommand(this.ApplyFilter);
        }

        [Reactive]
        public bool IsAvailable { get; set; } = true;

        [Reactive]
        public FilterItem FilterItem { get; set; } = null!;

        public ReactiveCommand<Unit, Func<ListItem, bool>> ApplyFilter { get; }
        public ReactiveCommand<Unit, Unit> ClearFilter { get; }

        private FilterItem WithSubscriptions(FilterItem filterItem)
            => filterItem switch
            {
                SimpleFilterItemViewModel simpleItem => this.WithSubscriptions(simpleItem),
                CompositeFilterItemViewModel simpleItem => this.WithSubscriptions(simpleItem),
                _ => filterItem
            };

        private SimpleFilterItemViewModel WithSubscriptions(SimpleFilterItemViewModel filterItem)
        {
            var subscriptions = new CompositeDisposable();

            filterItem.Delete
                .InvokeCommand(this.ClearFilter)
                .DisposeWith(subscriptions);

            filterItem.MakeComposite
                .Select(composition => this.WithSubscriptions(
                    CompositeFilterItemViewModel.FromSimpleItem(filterItem, composition)))
                .BindTo(this, vm => vm.FilterItem)
                .DisposeWith(subscriptions);

            filterItem.Delete
                .Merge(filterItem.MakeComposite.Discard())
                .Subscribe(subscriptions.Dispose)
                .DisposeWith(subscriptions);

            return filterItem;
        }

        private CompositeFilterItemViewModel WithSubscriptions(CompositeFilterItemViewModel filterItem)
        {
            var subscriptions = new CompositeDisposable();

            filterItem.Delete
                .InvokeCommand(this.ClearFilter)
                .DisposeWith(subscriptions);

            filterItem.Simplify
                .Select(this.WithSubscriptions)
                .BindTo(this, vm => vm.FilterItem)
                .DisposeWith(subscriptions);

            filterItem.Delete
                .Merge(filterItem.Simplify.Discard())
                .Subscribe(subscriptions.Dispose)
                .DisposeWith(subscriptions);

            return filterItem;
        }
    }
}
