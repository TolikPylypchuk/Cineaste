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
            [FilterType.Kind] = new()
            {
                FilterOperation.Is
            },
            [FilterType.Tags] = new()
            {
                FilterOperation.Include,
                FilterOperation.Exclude,
                FilterOperation.HaveCategory
            },
            [FilterType.Standalone] = new()
            {
                FilterOperation.None
            },
            [FilterType.Movie] = new()
            {
                FilterOperation.None
            },
            [FilterType.Series] = new()
            {
                FilterOperation.None
            }
        };

        private readonly SourceList<FilterOperation> availableOperationsSource = new();
        private readonly ReadOnlyObservableCollection<FilterOperation> availableOperations;

        private readonly ReadOnlyObservableCollection<Tag> tags;
        private ReadOnlyObservableCollection<string> tagCategories = null!;

        private ReadOnlyObservableCollection<string> kindNames = null!;

        public FilterItemViewModel(ReadOnlyObservableCollection<Kind> kinds, ReadOnlyObservableCollection<Tag> tags)
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

                kinds.ToObservableChangeSet()
                    .Transform(kind => kind.Name)
                    .Bind(out this.kindNames)
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
                    this.TitleIs(input.Text.ToLower()),

                (FilterType.Title, FilterOperation.StartsWith, TextFilterInputViewModel input) =>
                    this.TitleStartsWith(input.Text.ToLower()),

                (FilterType.Title, FilterOperation.EndsWith, TextFilterInputViewModel input) =>
                    this.TitleEndsWith(input.Text.ToLower()),

                (FilterType.Year, FilterOperation.Is, NumberFilterInputViewModel input) =>
                    this.YearIs(input.Number),

                (FilterType.Year, FilterOperation.LessThan, NumberFilterInputViewModel input) =>
                    this.YearLessThan(input.Number),

                (FilterType.Year, FilterOperation.GreaterThan, NumberFilterInputViewModel input) =>
                    this.YearGreaterThan(input.Number),

                (FilterType.Year, FilterOperation.Between, RangeFilterInputViewModel input) =>
                    input.Start <= input.End ? this.YearBetween(input.Start, input.End) : NoFilter,

                (FilterType.Kind, FilterOperation.Is, SelectionFilterInputViewModel input)
                    when input.SelectedItem != null =>
                    this.KindIs(input.SelectedItem),

                (FilterType.Standalone, _, _) =>
                   this.IsStandalone(),

                (FilterType.Movie, _, _) =>
                   this.IsMovie(),

                (FilterType.Series, _, _) =>
                   this.IsSeries(),

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

                (FilterType.Kind, FilterOperation.Is) =>
                    new SelectionFilterInputViewModel(this.kindNames) { Description = "Kind" },

                (FilterType.Tags, FilterOperation.Include or FilterOperation.Exclude) =>
                    new TagsFilterInputViewModel(this.tags) { Description = "Tags" },

                (FilterType.Tags, FilterOperation.HaveCategory) =>
                    new SelectionFilterInputViewModel(this.tagCategories) { Description = "TagCategory" },

                _ => new NoFilterInputViewModel()
            };

        private Func<ListItem, bool> TitleIs(string title)
            => String.IsNullOrEmpty(title) ? NoFilter : this.TitleFilter(t => t.Contains(title));

        private Func<ListItem, bool> TitleStartsWith(string title)
            => String.IsNullOrEmpty(title) ? NoFilter : this.TitleFilter(t => t.StartsWith(title));

        private Func<ListItem, bool> TitleEndsWith(string title)
            => String.IsNullOrEmpty(title) ? NoFilter : this.TitleFilter(t => t.EndsWith(title));

        private Func<ListItem, bool> TitleFilter(Func<string, bool> predicate)
            => this.CreateFilter(
                movie => movie.Titles.Any(t => predicate(t.Name.ToLower())),
                series => series.Titles.Any(t => predicate(t.Name.ToLower())));

        private Func<ListItem, bool> YearIs(int year)
            => this.CreateFilter(movie => movie.Year == year, series => this.SeriesYearIs(series, year));

        private bool SeriesYearIs(Series series, int year)
            => series.Seasons.Any(season => season.Periods
                .Any(period => period.StartYear <= year && period.EndYear >= year)) ||
                series.SpecialEpisodes.Any(episode => episode.Year == year);

        private Func<ListItem, bool> YearLessThan(int year)
            => this.CreateFilter(movie => movie.Year < year, series => series.StartYear < year);

        private Func<ListItem, bool> YearGreaterThan(int year)
            => this.CreateFilter(movie => movie.Year > year, series => series.EndYear > year);

        private Func<ListItem, bool> YearBetween(int startYear, int endYear)
            => this.CreateFilter(
                movie => startYear <= movie.Year && movie.Year <= endYear,
                series => this.SeriesYearBetween(series, startYear, endYear));

        private bool SeriesYearBetween(Series series, int startYear, int endYear)
            => series.Seasons.Any(season => season.Periods
                .Any(period => period.StartYear <= endYear && period.EndYear >= startYear)) ||
                series.SpecialEpisodes.Any(episode => startYear <= episode.Year && episode.Year <= endYear);

        private Func<ListItem, bool> KindIs(string kindName)
            => this.CreateFilter(movie => movie.Kind.Name == kindName, series => series.Kind.Name == kindName);

        private Func<ListItem, bool> IsStandalone()
            => item => item switch
            {
                MovieListItem movieItem => movieItem.Movie.Entry == null,
                SeriesListItem seriesItem => seriesItem.Series.Entry == null,
                _ => false
            };

        private Func<ListItem, bool> IsMovie()
            => this.CreateFilter(movie => true, series => false);

        private Func<ListItem, bool> IsSeries()
            => this.CreateFilter(movie => false, series => true);

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
