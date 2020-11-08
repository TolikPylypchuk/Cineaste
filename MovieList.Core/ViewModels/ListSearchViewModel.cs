using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using MovieList.Core.ListItems;
using MovieList.Core.ViewModels.Filters;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels
{
    public sealed class ListSearchViewModel : FilterItemHolder
    {
        private const int NoIndex = -1;

        private readonly ReadOnlyObservableCollection<ListItemViewModel> listItems;

        private readonly SourceList<ListItemViewModel> foundItemsSource = new();
        private readonly ReadOnlyObservableCollection<ListItemViewModel> foundItems;

        private bool shouldUpdateFilter;

        public ListSearchViewModel(
            ReadOnlyObservableCollection<ListItemViewModel> listItems,
            ReadOnlyObservableCollection<Kind> kinds,
            ReadOnlyObservableCollection<Tag> tags)
            : base(kinds, tags)
        {
            this.listItems = listItems;

            this.foundItemsSource.Connect()
                .Bind(out this.foundItems)
                .DisposeMany()
                .Subscribe();

            this.listItems.ToObservableChangeSet()
                .Discard()
                .Subscribe(this.ShouldUpdateFilter);

            this.FindNext = ReactiveCommand.Create(this.OnFindNext);
            this.FindPrevious = ReactiveCommand.Create(this.OnFindPrevious);
            this.StopSearch = ReactiveCommand.Create(this.OnStopSearch);

            this.Clear.InvokeCommand(this.StopSearch);
            this.Clear.Subscribe(this.foundItemsSource.Clear);

            this.FindNext.Select(_ => true)
                .Merge(this.FindPrevious.Select(_ => true))
                .Merge(this.StopSearch.Select(_ => false))
                .Merge(this.Clear.Select(_ => false))
                .ToPropertyEx(this, v => v.IsSearchInitialized, initialValue: false);
        }

        [Reactive]
        public int CurrentIndex { get; private set; } = NoIndex;

        [Reactive]
        public int TotalSearchedItemsCount { get; private set; } = 0;

        public bool IsSearchInitialized { [ObservableAsProperty] get; }

        public ReadOnlyObservableCollection<ListItemViewModel> FoundItems =>
            this.foundItems;

        public ReactiveCommand<Unit, ListItemViewModel?> FindNext { get; }
        public ReactiveCommand<Unit, ListItemViewModel?> FindPrevious { get; }
        public ReactiveCommand<Unit, Unit> StopSearch { get; }

        protected override void AddSubscriptions(
            SimpleFilterItemViewModel filterItem,
            CompositeDisposable subscriptions)
        {
            this.SubscribeToChange(filterItem, subscriptions);
            base.AddSubscriptions(filterItem, subscriptions);
        }

        protected override void AddSubscriptions(
            CompositeFilterItemViewModel filterItem,
            CompositeDisposable subscriptions)
        {
            this.SubscribeToChange(filterItem, subscriptions);
            base.AddSubscriptions(filterItem, subscriptions);
        }

        private ListItemViewModel? OnFindNext() =>
            this.Find(() => (this.CurrentIndex + 1 + this.foundItems.Count) % this.foundItems.Count);

        private ListItemViewModel? OnFindPrevious() =>
            this.Find(() => (Math.Max(this.CurrentIndex, 0) - 1 + this.foundItems.Count) % this.foundItems.Count);

        private void OnStopSearch()
        {
            this.RemoveHighlights();
            this.CurrentIndex = NoIndex;
            this.ShouldUpdateFilter();
        }

        private ListItemViewModel? Find(Func<int> nextIndex)
        {
            if (this.shouldUpdateFilter)
            {
                this.UpdateFilter();
            }

            if (this.foundItems.Count == 0 || this.foundItems.Count == this.TotalSearchedItemsCount)
            {
                return null;
            }

            if (this.CurrentIndex != NoIndex)
            {
                this.foundItems[this.CurrentIndex].Item.HighlightMode = HighlightMode.Partial;
            }

            this.CurrentIndex = nextIndex();

            var result = this.foundItems[this.CurrentIndex];
            result.Item.HighlightMode = HighlightMode.Full;

            return result;
        }

        private void UpdateFilter()
        {
            var filter = this.FilterItem.CreateFilter();

            this.RemoveHighlights();

            var newItems = this.listItems
                .Where(item => item.Item is not FranchiseListItem && filter(item.Item))
                .ToList();

            this.TotalSearchedItemsCount = this.listItems.Count(item => item.Item is not FranchiseListItem);

            if (newItems.Count != this.TotalSearchedItemsCount)
            {
                foreach (var item in newItems)
                {
                    item.Item.HighlightMode = HighlightMode.Partial;
                }
            }

            this.foundItemsSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(newItems);
            });

            this.CurrentIndex = NoIndex;
            this.shouldUpdateFilter = false;
        }

        private void RemoveHighlights()
        {
            foreach (var item in this.foundItems)
            {
                item.Item.HighlightMode = HighlightMode.None;
            }
        }

        private void ShouldUpdateFilter() =>
            this.shouldUpdateFilter = true;

        private void SubscribeToChange(FilterItem filterItem, CompositeDisposable subscriptions) =>
            filterItem.FilterChanged
                .Subscribe(this.ShouldUpdateFilter)
                .DisposeWith(subscriptions);
    }
}
