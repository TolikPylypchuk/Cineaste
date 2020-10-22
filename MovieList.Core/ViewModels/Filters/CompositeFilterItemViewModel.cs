using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;

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
                .Bind(out items)
                .DisposeMany()
                .Subscribe();

            this.SwitchComposition = ReactiveCommand.Create<Unit, FilterComposition>(
                _ => this.Composition == FilterComposition.And ? FilterComposition.Or : FilterComposition.And);

            this.AddItem = ReactiveCommand.Create(this.OnAddItem);
            this.Delete = ReactiveCommand.Create(() => { });

            this.SwitchComposition
                .ToPropertyEx(this, vm => vm.Composition, initialValue: initialComposition);
        }

        public FilterComposition Composition { [ObservableAsProperty] get; }

        public ReadOnlyObservableCollection<FilterItem> Items
            => this.items;

        public ReactiveCommand<Unit, FilterComposition> SwitchComposition { get; }
        public ReactiveCommand<Unit, Unit> AddItem { get; }
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

            result.CreateSubscriptions(simpleItem);
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

            return this.Items.Select(item => item.CreateFilter()).Aggregate(composeFilters);
        }

        private void OnAddItem()
        {
            var simpleItem = new SimpleFilterItemViewModel(
                this.Kinds, this.Tags, this.SeriesWatchStatusConverter, this.SeriesReleaseStatusConverter);

            this.CreateSubscriptions(simpleItem);
            this.itemsSource.Add(simpleItem);
        }

        private void CreateSubscriptions(FilterItem item)
        {
            var subscriptions = new CompositeDisposable();

            if (item is SimpleFilterItemViewModel simpleItem)
            {
                simpleItem.MakeComposite
                    .Select(composition => FromSimpleItem(simpleItem, composition))
                    .Subscribe(compositeItem =>
                    {
                        var index = this.itemsSource.Items.IndexOf(simpleItem);
                        this.itemsSource.ReplaceAt(index, compositeItem);
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
        }
    }
}
