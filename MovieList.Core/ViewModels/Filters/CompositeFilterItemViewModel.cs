using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Aggregation;

using MovieList.Core.Data.Models;
using MovieList.Core.ListItems;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Filter = System.Func<MovieList.Core.ListItems.ListItem, bool>;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class CompositeFilterItemViewModel : FilterItem
    {
        private readonly SourceList<FilterItem> itemsSource = new();
        private readonly ReadOnlyObservableCollection<FilterItem> items;

        public CompositeFilterItemViewModel(
            ReadOnlyObservableCollection<Kind> kinds,
            ReadOnlyObservableCollection<Tag> tags,
            FilterComposition initialComposition = FilterComposition.And,
            IEnumConverter<SeriesWatchStatus>? seriesWatchStatusConverter = null,
            IEnumConverter<SeriesReleaseStatus>? seriesReleaseStatusConverter = null)
            : base(kinds, tags, seriesWatchStatusConverter, seriesReleaseStatusConverter)
        {
            this.itemsSource.Connect()
                .Transform(this.WithSubscriptions)
                .Bind(out items)
                .DisposeMany()
                .Subscribe();

            this.SwitchComposition = ReactiveCommand.Create<Unit, FilterComposition>(
                _ => this.Composition == FilterComposition.And ? FilterComposition.Or : FilterComposition.And);

            this.AddItem = ReactiveCommand.Create(this.OnAddItem);

            var canSimplify = this.itemsSource.Connect()
                .Count()
                .Select(count => count == 1);

            this.Simplify = ReactiveCommand.Create(() => this.Items[0], canSimplify);
            this.Delete = ReactiveCommand.Create(() => { });

            this.SwitchComposition
                .ToPropertyEx(this, vm => vm.Composition, initialValue: initialComposition);
        }

        public FilterComposition Composition { [ObservableAsProperty] get; }

        public ReadOnlyObservableCollection<FilterItem> Items
            => this.items;

        public ReactiveCommand<Unit, FilterComposition> SwitchComposition { get; }
        public ReactiveCommand<Unit, Unit> AddItem { get; }
        public ReactiveCommand<Unit, FilterItem> Simplify { get; }
        public override ReactiveCommand<Unit, Unit> Delete { get; }

        public static CompositeFilterItemViewModel FromSimpleItem(
            SimpleFilterItemViewModel simpleItem,
            FilterComposition initialComposition)
        {
            var result = new CompositeFilterItemViewModel(
                simpleItem.Kinds,
                simpleItem.Tags,
                initialComposition,
                simpleItem.SeriesWatchStatusConverter,
                simpleItem.SeriesReleaseStatusConverter);

            result.itemsSource.Add(simpleItem);

            return result;
        }

        public override Filter CreateFilter()
        {
            if (this.Items.Count == 0)
            {
                return NoFilter;
            }

            Func<Filter, Filter, Filter> composeFilters = this.Composition switch
            {
                FilterComposition.And => (a, b) => item => a(item) && b(item),
                FilterComposition.Or => (a, b) => item => a(item) || b(item),
                _ => (_, _) => NoFilter
            };

            var filters = this.Items.Select(item => item.CreateFilter()).Aggregate(composeFilters);

            Filter filter = null!;

            filter = item => item switch
            {
                MovieListItem movieItem => filters(movieItem),
                SeriesListItem seriesItem => filters(seriesItem),
                FranchiseListItem franchiseItem =>
                    franchiseItem.Franchise.ShowTitles &&
                    franchiseItem.Franchise.Entries
                        .Select(entry => entry.ToListItem())
                        .Any(item => filter(item)),
                _ => false
            };

            return filter;
        }

        private void OnAddItem()
            => this.itemsSource.Add(new SimpleFilterItemViewModel(
                this.Kinds, this.Tags, this.SeriesWatchStatusConverter, this.SeriesReleaseStatusConverter));

        private FilterItem WithSubscriptions(FilterItem item)
        {
            var subscriptions = new CompositeDisposable();

            if (item is SimpleFilterItemViewModel simpleItem)
            {
                simpleItem.MakeComposite
                    .Select(composition => FromSimpleItem(simpleItem, composition))
                    .Subscribe(compositeItem =>
                    {
                        this.itemsSource.Replace(simpleItem, compositeItem);
                        subscriptions.Dispose();
                    })
                    .DisposeWith(subscriptions);
            } else if (item is CompositeFilterItemViewModel compositeItem)
            {
                compositeItem.Simplify
                    .Subscribe(newItem =>
                    {
                        this.itemsSource.Replace(compositeItem, newItem);
                        subscriptions.Dispose();
                    })
                    .DisposeWith(subscriptions);
            }

            item.Delete
                .Subscribe(() =>
                {
                    this.itemsSource.Remove(item);
                    subscriptions.Dispose();
                })
                .DisposeWith(subscriptions);

            return item;
        }
    }
}
