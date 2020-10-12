using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using MovieList.Core.Data.Models;
using MovieList.Core.ListItems;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class FilterItemViewModel : ReactiveObject, IActivatableViewModel
    {
        private static readonly Dictionary<FilterType, List<FilterOperation>> AvailableOperationsByType = new()
        {
            [FilterType.Title] = new()
            {
                FilterOperation.Is,
                FilterOperation.StartsWith,
                FilterOperation.EndsWith
            },
            [FilterType.Year] = new()
            {
                FilterOperation.Is,
                FilterOperation.LessThan,
                FilterOperation.GreaterThan,
                FilterOperation.Between
            },
            [FilterType.Tags] = new()
            {
                FilterOperation.Include,
                FilterOperation.Exclude,
                FilterOperation.HaveCategory,
                FilterOperation.NoneOfCategory
            },
            [FilterType.Standalone] = new()
            {
                FilterOperation.None
            }
        };

        private readonly SourceList<FilterOperation> availableOperationsSource = new();
        private readonly ReadOnlyObservableCollection<FilterOperation> availableOperations;

        private readonly ReadOnlyObservableCollection<Tag> tags;
        private ReadOnlyObservableCollection<string> tagCategories = null!;

        public FilterItemViewModel(ReadOnlyObservableCollection<Tag> tags)
        {
            this.tags = tags;

            this.availableOperationsSource.Connect()
                .Bind(out this.availableOperations)
                .Subscribe();

            this.WhenAnyValue(vm => vm.FilterType)
                .Select(ft => AvailableOperationsByType[ft])
                .Subscribe(ops =>
                {
                    this.availableOperationsSource.Edit(list =>
                    {
                        list.Clear();
                        list.AddRange(ops);
                    });

                    this.FilterOperation = ops[0];
                });

            this.WhenAnyValue(vm => vm.FilterType)
                .Discard()
                .Select(this.CreateFilterInput)
                .ToPropertyEx(this, vm => vm.FilterInput, initialValue: new TextFilterInputViewModel());

            this.WhenActivated(disposables =>
            {
                tags.ToObservableChangeSet()
                    .Transform(tag => tag.Category)
                    .DistinctValues(c => c)
                    .Sort(Comparer<string>.Default)
                    .Bind(out this.tagCategories)
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        [Reactive]
        public FilterType FilterType { get; set; } = FilterType.Title;

        [Reactive]
        public FilterOperation FilterOperation { get; set; } = FilterOperation.Is;

        public FilterInput FilterInput { [ObservableAsProperty] get; } = null!;

        public ReadOnlyObservableCollection<FilterOperation> AvailableOperations
            => this.availableOperations;

        [Reactive]
        public string Text { get; set; } = String.Empty;

        [Reactive]
        public int Number { get; set; }

        [Reactive]
        public int RangeStart { get; set; }

        [Reactive]
        public int RangeEnd { get; set; }

        [Reactive]
        public bool IsChecked { get; set; }

        public Func<ListItem, bool> CreateFilter()
            => (this.FilterType, this.FilterOperation, this.FilterInput) switch
            {
                (FilterType.Title, FilterOperation.Is, TextFilterInputViewModel input) =>
                    item => this.TitleIs(input.Text.ToLower(), item),
                (FilterType.Title, FilterOperation.StartsWith, TextFilterInputViewModel input) =>
                    item => this.TitleStartsWith(input.Text.ToLower(), item),
                (FilterType.Title, FilterOperation.EndsWith, TextFilterInputViewModel input) =>
                    item => this.TitleEndsWith(input.Text.ToLower(), item),
                _ => item => true
            };

        private FilterInput CreateFilterInput()
            => (this.FilterType, this.FilterOperation) switch
            {
                (FilterType.Title, _) =>
                    new TextFilterInputViewModel { Description = "Title" },
                (FilterType.Year, FilterOperation.Is or FilterOperation.LessThan or FilterOperation.GreaterThan) =>
                    new NumberFilterInputViewModel { Description = "Year" },
                (FilterType.Year, FilterOperation.Between) =>
                    new RangeFilterInputViewModel { Description = "Year" },
                (FilterType.Tags, FilterOperation.Include or FilterOperation.Exclude) =>
                    new TagsFilterInputViewModel(this.tags) { Description = "Tags" },
                (FilterType.Tags, FilterOperation.HaveCategory or FilterOperation.NoneOfCategory) =>
                    new SelectionFilterInputViewModel(this.tagCategories) { Description = "TagCategory" },
                (FilterType.Standalone, FilterOperation.None) =>
                    new BooleanFilterInputViewModel { Description = "Standalone" },
                _ =>
                    throw new ArgumentException(
                        $"Cannot create filter input for {this.FilterType}, {this.FilterOperation}")
            };

        private bool TitleIs(string title, ListItem item)
            => String.IsNullOrEmpty(title) || this.FilterTitle(t => t.Contains(title), item);

        private bool TitleStartsWith(string title, ListItem item)
            => String.IsNullOrEmpty(title) || this.FilterTitle(t => t.StartsWith(title), item);

        private bool TitleEndsWith(string title, ListItem item)
            => String.IsNullOrEmpty(title) || this.FilterTitle(t => t.EndsWith(title), item);

        private bool FilterTitle(Func<string, bool> predicate, ListItem item)
            => item switch
            {
                MovieListItem movieItem => movieItem.Movie.Titles.Any(t => predicate(t.Name.ToLower())),
                SeriesListItem seriesItem => seriesItem.Series.Titles.Any(t => predicate(t.Name.ToLower())),
                FranchiseListItem franchiseItem =>
                    franchiseItem.Franchise.ShowTitles &&
                    franchiseItem.Franchise.Entries
                        .Select(entry => entry.ToListItem())
                        .Any(item => this.FilterTitle(predicate, item)),
                _ => false
            };
    }
}
