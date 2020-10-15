using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

using DynamicData;
using DynamicData.Binding;

using MovieList.Core.Data.Models;
using MovieList.Core.ListItems;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using static MovieList.Core.Constants;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class FilterItemViewModel : ReactiveObject, IActivatableViewModel
    {
        private static readonly Func<ListItem, bool> NoFilter = item => true;

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
                });

            this.WhenAnyValue(vm => vm.FilterType)
                .Select(_ => this.AvailableOperations[0])
                .Subscribe(op =>
                {
                    this.FilterOperation = FilterOperation.None;
                    this.FilterOperation = op;
                });

            this.WhenAnyValue(vm => vm.FilterOperation)
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
                    this.CreateTitleIs(input.Text.ToLower()),

                (FilterType.Title, FilterOperation.StartsWith, TextFilterInputViewModel input) =>
                    this.CreateTitleStartsWith(input.Text.ToLower()),

                (FilterType.Title, FilterOperation.EndsWith, TextFilterInputViewModel input) =>
                    this.CreateTitleEndsWith(input.Text.ToLower()),

                (FilterType.Year, FilterOperation.Is, NumberFilterInputViewModel input) =>
                    this.CreateYearIs(input.Number),

                (FilterType.Year, FilterOperation.LessThan, NumberFilterInputViewModel input) =>
                    this.CreateYearLessThan(input.Number),

                (FilterType.Year, FilterOperation.GreaterThan, NumberFilterInputViewModel input) =>
                    this.CreateYearGreaterThan(input.Number),

                (FilterType.Year, FilterOperation.Between, RangeFilterInputViewModel input) =>
                    input.Start <= input.End ? this.CreateYearBetween(input.Start, input.End) : NoFilter,

                _ => NoFilter
            };

        private FilterInput CreateFilterInput()
            => (this.FilterType, this.FilterOperation) switch
            {
                (FilterType.Title, _) =>
                    new TextFilterInputViewModel { Description = "Title" },

                (FilterType.Year, FilterOperation.Is or FilterOperation.LessThan or FilterOperation.GreaterThan) =>
                    new NumberFilterInputViewModel { Description = "Year", Number = DefaultFilterYearValue },

                (FilterType.Year, FilterOperation.Between) =>
                    new RangeFilterInputViewModel
                    {
                        Description = "Year",
                        Start = DefaultFilterYearValue,
                        End = DefaultFilterYearValue
                    },

                (FilterType.Tags, FilterOperation.Include or FilterOperation.Exclude) =>
                    new TagsFilterInputViewModel(this.tags) { Description = "Tags" },

                (FilterType.Tags, FilterOperation.HaveCategory or FilterOperation.NoneOfCategory) =>
                    new SelectionFilterInputViewModel(this.tagCategories) { Description = "TagCategory" },

                _ => new NoFilterInputViewModel()
            };

        private Func<ListItem, bool> CreateTitleIs(string title)
            => String.IsNullOrEmpty(title) ? NoFilter : this.CreateTitleFilter(t => t.Contains(title));

        private Func<ListItem, bool> CreateTitleStartsWith(string title)
            => String.IsNullOrEmpty(title) ? NoFilter : this.CreateTitleFilter(t => t.StartsWith(title));

        private Func<ListItem, bool> CreateTitleEndsWith(string title)
            => String.IsNullOrEmpty(title) ? NoFilter : this.CreateTitleFilter(t => t.EndsWith(title));

        private Func<ListItem, bool> CreateTitleFilter(Func<string, bool> predicate)
            => this.CreateFilter(
                movie => movie.Titles.Any(t => predicate(t.Name.ToLower())),
                series => series.Titles.Any(t => predicate(t.Name.ToLower())));

        private Func<ListItem, bool> CreateYearIs(int year)
            => this.CreateFilter(movie => movie.Year == year, series => this.SeriesYearIs(series, year));

        private Func<ListItem, bool> CreateYearLessThan(int year)
            => this.CreateFilter(movie => movie.Year < year, series => series.StartYear < year);

        private Func<ListItem, bool> CreateYearGreaterThan(int year)
            => this.CreateFilter(movie => movie.Year > year, series => series.EndYear > year);

        private Func<ListItem, bool> CreateYearBetween(int startYear, int endYear)
            => this.CreateFilter(
                movie => startYear <= movie.Year && movie.Year <= endYear,
                series => this.SeriesYearBetween(series, startYear, endYear));

        private bool SeriesYearIs(Series series, int year)
            => series.Seasons.Any(season => season.Periods
                .Any(period => period.StartYear <= year && period.EndYear >= year)) ||
                series.SpecialEpisodes.Any(episode => episode.Year == year);

        private bool SeriesYearBetween(Series series, int startYear, int endYear)
            => series.Seasons.Any(season => season.Periods
                .Any(period => period.StartYear <= endYear && period.EndYear >= startYear)) ||
                series.SpecialEpisodes.Any(episode => startYear <= episode.Year && episode.Year <= endYear);

        private Func<ListItem, bool> CreateFilter(Func<Movie, bool> moviePredicate, Func<Series, bool> seriesPredicate)
        {
            Func<ListItem, bool> filter = null!;
            filter = item => item switch
            {
                MovieListItem movieItem => moviePredicate(movieItem.Movie),
                SeriesListItem seriesItem => seriesPredicate(seriesItem.Series),
                FranchiseListItem franchiseItem =>
                    franchiseItem.Franchise.ShowTitles &&
                    franchiseItem.Franchise.Entries
                        .Select(entry => entry.ToListItem())
                        .Any(item => filter(item)),
                _ => false
            };

            return filter;
        }
    }
}
