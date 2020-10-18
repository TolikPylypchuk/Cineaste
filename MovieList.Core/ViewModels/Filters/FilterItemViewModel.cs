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
using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using static MovieList.Core.Constants;
using static MovieList.Core.ViewModels.Filters.FilterOperation;
using static MovieList.Core.ViewModels.Filters.FilterType;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class FilterItemViewModel : ReactiveObject, IActivatableViewModel
    {
        private const FilterType Title = FilterType.Title;
        private const FilterType Kind = FilterType.Kind;
        private const FilterType Movie = FilterType.Movie;
        private const FilterType Series = FilterType.Series;
        private const FilterType SeriesWatchStatus = FilterType.SeriesWatchStatus;
        private const FilterType SeriesReleaseStatus = FilterType.SeriesReleaseStatus;

        private static readonly Func<ListItem, bool> NoFilter = item => true;

        private readonly IEnumConverter<SeriesWatchStatus> seriesWatchStatusConverter;
        private readonly IEnumConverter<SeriesReleaseStatus> seriesReleaseStatusConverter;

        private readonly SourceList<FilterOperation> availableOperationsSource = new();
        private readonly ReadOnlyObservableCollection<FilterOperation> availableOperations;

        private readonly ReadOnlyObservableCollection<Tag> tags;

        private ReadOnlyObservableCollection<string> tagCategories = null!;
        private ReadOnlyObservableCollection<string> kindNames = null!;
        private readonly ReadOnlyObservableCollection<string> seriesWatchStatuses = null!;
        private readonly ReadOnlyObservableCollection<string> seriesReleaseStatuses = null!;

        public FilterItemViewModel(
            ReadOnlyObservableCollection<Kind> kinds,
            ReadOnlyObservableCollection<Tag> tags,
            IEnumConverter<SeriesWatchStatus>? seriesWatchStatusConverter = null,
            IEnumConverter<SeriesReleaseStatus>? seriesReleaseStatusConverter = null)
        {
            this.tags = tags;

            this.seriesWatchStatusConverter = seriesWatchStatusConverter
                ?? Locator.Current.GetService<IEnumConverter<SeriesWatchStatus>>();

            this.seriesReleaseStatusConverter = seriesReleaseStatusConverter
                ?? Locator.Current.GetService<IEnumConverter<SeriesReleaseStatus>>();

            this.availableOperationsSource.Connect()
                .Bind(out this.availableOperations)
                .Subscribe();

            this.WhenAnyValue(vm => vm.FilterType)
                .Select(ft => FilterOperations.ByType[ft])
                .Subscribe(ops =>
                    this.availableOperationsSource.Edit(list =>
                    {
                        list.Clear();
                        list.AddRange(ops);
                    }));

            this.WhenAnyValue(vm => vm.FilterType)
                .Select(_ => this.availableOperations[0])
                .Subscribe(op =>
                {
                    this.FilterOperation = None;
                    this.FilterOperation = op;
                });

            this.WhenAnyValue(vm => vm.FilterOperation)
                .Discard()
                .Select(this.CreateFilterInput)
                .ToPropertyEx(this, vm => vm.FilterInput, initialValue: new TextFilterInputViewModel());

            var seriesWatchStatuses = new ObservableCollection<string>();

            seriesWatchStatuses.AddRange(
                Enum.GetValues<SeriesWatchStatus>().Select(this.seriesWatchStatusConverter.ToString));

            var seriesReleaseStatuses = new ObservableCollection<string>();

            seriesReleaseStatuses.AddRange(
                Enum.GetValues<SeriesReleaseStatus>().Select(this.seriesReleaseStatusConverter.ToString));

            this.seriesWatchStatuses = new ReadOnlyObservableCollection<string>(seriesWatchStatuses);
            this.seriesReleaseStatuses = new ReadOnlyObservableCollection<string>(seriesReleaseStatuses);

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

        [Reactive]
        public FilterType FilterType { get; set; } = Title;

        [Reactive]
        public FilterOperation FilterOperation { get; set; } = Is;

        public FilterInput FilterInput { [ObservableAsProperty] get; } = null!;

        [Reactive]
        public bool IsNegated { get; set; }

        public ReadOnlyObservableCollection<FilterOperation> AvailableOperations
            => this.availableOperations;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public Func<ListItem, bool> CreateFilter()
        {
            var coreFilter = this.CreateCoreFilter();
            return this.IsNegated ? item => !coreFilter(item) : coreFilter;
        }

        private Func<ListItem, bool> CreateCoreFilter()
            => (this.FilterType, this.FilterOperation, this.FilterInput) switch
            {
                (Title, Is, TextFilterInputViewModel input) => this.TitleIs(input.Text),

                (Title, StartsWith, TextFilterInputViewModel input) => this.TitleStartsWith(input.Text),

                (Title, EndsWith, TextFilterInputViewModel input) => this.TitleEndsWith(input.Text),

                (Year, Is, NumberFilterInputViewModel input) => this.YearIs(input.Number),

                (Year, LessThan, NumberFilterInputViewModel input) => this.YearLessThan(input.Number),

                (Year, GreaterThan, NumberFilterInputViewModel input) => this.YearGreaterThan(input.Number),

                (Year, Between, RangeFilterInputViewModel input) => this.YearBetween(input.Start, input.End),

                (Kind, Is, SelectionFilterInputViewModel input) => this.KindIs(input.SelectedItem),

                (Tags, Include, TagsFilterInputViewModel input) => this.TagsInclude(input.Tags),

                (Tags, Exclude, TagsFilterInputViewModel input) => this.TagsExclude(input.Tags),

                (Tags, HaveCategory, SelectionFilterInputViewModel input) => this.TagsHaveCategory(input.SelectedItem),

                (Standalone, _, _) => this.IsStandalone(),

                (Movie, _, _) => this.IsMovie(),

                (MovieWatched, _, _) => this.IsMovieWatched(),

                (MovieReleased, _, _) => this.IsMovieReleased(),

                (Series, _, _) => this.IsSeries(),

                (SeriesWatchStatus, Is, SelectionFilterInputViewModel input) =>
                    this.SeriesWatchStatusIs(input.SelectedItem),

                (SeriesReleaseStatus, Is, SelectionFilterInputViewModel input) =>
                    this.SeriesReleaseStatusIs(input.SelectedItem),

                (SeriesChannel, Is, TextFilterInputViewModel input) => this.SeriesChannelIs(input.Text),

                (SeriesChannel, StartsWith, TextFilterInputViewModel input) => this.SeriesChannelStartsWith(input.Text),

                (SeriesChannel, EndsWith, TextFilterInputViewModel input) => this.SeriesChannelEndsWith(input.Text),

                (SeriesNumberOfSeasons, Is, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfSeasonsIs(input.Number),

                (SeriesNumberOfSeasons, LessThan, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfSeasonsLessThan(input.Number),

                (SeriesNumberOfSeasons, GreaterThan, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfSeasonsGreaterThan(input.Number),

                (SeriesNumberOfSeasons, Between, RangeFilterInputViewModel input) =>
                    this.SeriesNumberOfSeasonsBetween(input.Start, input.End),

                (SeriesNumberOfEpisodes, Is, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfEpisodesIs(input.Number),

                (SeriesNumberOfEpisodes, LessThan, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfEpisodesLessThan(input.Number),

                (SeriesNumberOfEpisodes, GreaterThan, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfEpisodesGreaterThan(input.Number),

                (SeriesNumberOfEpisodes, Between, RangeFilterInputViewModel input) =>
                    this.SeriesNumberOfEpisodesBetween(input.Start, input.End),

                (SeriesMiniseries, _, _) => this.SeriesIsMiniseries(),

                (SeriesAnthology, _, _) => this.SeriesIsAnthology(),

                _ => NoFilter
            };

        private FilterInput CreateFilterInput()
            => (this.FilterType, this.FilterOperation) switch
            {
                (Title, _) =>
                    new TextFilterInputViewModel { Description = nameof(Title) },

                (Year, Is or LessThan or GreaterThan) =>
                    new NumberFilterInputViewModel { Description = nameof(Year), Number = DefaultFilterYearValue },

                (Year, Between) =>
                    new RangeFilterInputViewModel
                    {
                        Description = nameof(Year),
                        Start = DefaultFilterYearValue,
                        End = DefaultFilterYearValue
                    },

                (Kind, Is) =>
                    new SelectionFilterInputViewModel(this.kindNames) { Description = nameof(Kind) },

                (Tags, Include or Exclude) =>
                    new TagsFilterInputViewModel(this.tags) { Description = nameof(Tags) },

                (Tags, HaveCategory) =>
                    new SelectionFilterInputViewModel(this.tagCategories) { Description = "TagCategory" },

                (SeriesWatchStatus, _) =>
                    new SelectionFilterInputViewModel(this.seriesWatchStatuses)
                    { Description = nameof(SeriesWatchStatus) },

                (SeriesReleaseStatus, _) =>
                    new SelectionFilterInputViewModel(this.seriesReleaseStatuses)
                    { Description = nameof(SeriesReleaseStatus) },

                (SeriesChannel, _) =>
                    new TextFilterInputViewModel { Description = nameof(SeriesChannel) },

                (SeriesNumberOfSeasons, Is or LessThan or GreaterThan) =>
                    new NumberFilterInputViewModel
                    {
                        Description = nameof(SeriesNumberOfSeasons),
                        Number = DefaultFilterNumberOfSeasonsValue
                    },

                (SeriesNumberOfSeasons, Between) =>
                    new RangeFilterInputViewModel
                    {
                        Description = nameof(SeriesNumberOfSeasons),
                        Start = DefaultFilterNumberOfSeasonsValue,
                        End = DefaultFilterNumberOfSeasonsValue
                    },

                (SeriesNumberOfEpisodes, Is or LessThan or GreaterThan) =>
                    new NumberFilterInputViewModel
                    {
                        Description = nameof(SeriesNumberOfEpisodes),
                        Number = DefaultFilterNumberOfSeasonsValue
                    },

                (SeriesNumberOfEpisodes, Between) =>
                    new RangeFilterInputViewModel
                    {
                        Description = nameof(SeriesNumberOfEpisodes),
                        Start = DefaultFilterNumberOfSeasonsValue,
                        End = DefaultFilterNumberOfSeasonsValue
                    },

                _ => new NoFilterInputViewModel()
            };

        private Func<ListItem, bool> TitleIs(string? title)
            => !String.IsNullOrEmpty(title)
                ? this.TitleFilter(t => t.Contains(title, StringComparison.InvariantCultureIgnoreCase))
                : NoFilter;

        private Func<ListItem, bool> TitleStartsWith(string? title)
            => !String.IsNullOrEmpty(title)
                ? this.TitleFilter(t => t.StartsWith(title, StringComparison.InvariantCultureIgnoreCase))
                : NoFilter;

        private Func<ListItem, bool> TitleEndsWith(string? title)
            => !String.IsNullOrEmpty(title)
                ? this.TitleFilter(t => t.EndsWith(title, StringComparison.InvariantCultureIgnoreCase))
                : NoFilter;

        private Func<ListItem, bool> TitleFilter(Func<string, bool> predicate)
            => this.CreateFilter(
                movie => movie.Titles.Any(t => predicate(t.Name)),
                series => series.Titles.Any(t => predicate(t.Name)));

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
            => startYear <= endYear
                ? this.CreateFilter(
                    movie => startYear <= movie.Year && movie.Year <= endYear,
                    series => this.SeriesYearBetween(series, startYear, endYear))
                : NoFilter;

        private bool SeriesYearBetween(Series series, int startYear, int endYear)
            => series.Seasons.Any(season => season.Periods
                .Any(period => period.StartYear <= endYear && period.EndYear >= startYear)) ||
                series.SpecialEpisodes.Any(episode => startYear <= episode.Year && episode.Year <= endYear);

        private Func<ListItem, bool> KindIs(string? kindName)
            => !String.IsNullOrEmpty(kindName)
                ? this.CreateFilter(movie => movie.Kind.Name == kindName, series => series.Kind.Name == kindName)
                : NoFilter;

        private Func<ListItem, bool> TagsInclude(IEnumerable<TagItemViewModel> tagItems)
            => this.CreateTagFilter(
                tagItems,
                movie => tags.All(tag => movie.Tags.Contains(tag)),
                series => tags.All(tag => series.Tags.Contains(tag)));

        private Func<ListItem, bool> TagsExclude(IEnumerable<TagItemViewModel> tagItems)
            => this.CreateTagFilter(
                tagItems,
                movie => tags.All(tag => !movie.Tags.Contains(tag)),
                series => tags.All(tag => !series.Tags.Contains(tag)));

        private Func<ListItem, bool> CreateTagFilter(
            IEnumerable<TagItemViewModel> tagItems,
            Func<Movie, bool> moviePredicate,
            Func<Series, bool> seriesPredicate)
        {
            var tags = tagItems.Select(item => item.Tag).ToList();
            return !tags.IsEmpty() ? this.CreateFilter(moviePredicate, seriesPredicate) : NoFilter;
        }

        private Func<ListItem, bool> TagsHaveCategory(string? category)
            => !String.IsNullOrEmpty(category)
                ? this.CreateFilter(
                    movie => movie.Tags.Any(tag => tag.Category == category),
                    series => series.Tags.Any(tag => tag.Category == category))
                : NoFilter;

        private Func<ListItem, bool> IsStandalone()
            => item => item switch
            {
                MovieListItem movieItem => movieItem.Movie.Entry == null,
                SeriesListItem seriesItem => seriesItem.Series.Entry == null,
                _ => false
            };

        private Func<ListItem, bool> IsMovie()
            => this.CreateMovieFilter(movie => true);

        private Func<ListItem, bool> IsMovieWatched()
            => this.CreateMovieFilter(movie => movie.IsWatched);

        private Func<ListItem, bool> IsMovieReleased()
            => this.CreateMovieFilter(movie => movie.IsReleased);

        private Func<ListItem, bool> CreateMovieFilter(Func<Movie, bool> predicate)
            => this.CreateFilter(predicate, series => false);

        private Func<ListItem, bool> IsSeries()
            => this.CreateSeriesFilter(series => true);

        private Func<ListItem, bool> SeriesWatchStatusIs(string? status)
            => !String.IsNullOrEmpty(status)
                ? this.CreateSeriesFilter(
                    series => this.seriesWatchStatusConverter.ToString(series.WatchStatus) == status)
                : NoFilter;

        private Func<ListItem, bool> SeriesReleaseStatusIs(string? status)
            => !String.IsNullOrEmpty(status)
                ? this.CreateSeriesFilter(
                    series => this.seriesReleaseStatusConverter.ToString(series.ReleaseStatus) == status)
                : NoFilter;

        private Func<ListItem, bool> SeriesChannelIs(string? channel)
            => !String.IsNullOrEmpty(channel)
                ? this.CreateSeriesFilter(series =>
                    series.Seasons.Any(season =>
                        season.Channel.Equals(channel, StringComparison.InvariantCultureIgnoreCase)) ||
                    series.SpecialEpisodes.Any(episode =>
                        episode.Channel.Equals(channel, StringComparison.InvariantCultureIgnoreCase)))
                : NoFilter;

        private Func<ListItem, bool> SeriesChannelStartsWith(string? channel)
            => !String.IsNullOrEmpty(channel)
                ? this.CreateSeriesFilter(series =>
                    series.Seasons.Any(season =>
                        season.Channel.StartsWith(channel, StringComparison.InvariantCultureIgnoreCase)) ||
                    series.SpecialEpisodes.Any(episode =>
                        episode.Channel.StartsWith(channel, StringComparison.InvariantCultureIgnoreCase)))
                : NoFilter;

        private Func<ListItem, bool> SeriesChannelEndsWith(string? channel)
            => !String.IsNullOrEmpty(channel)
                ? this.CreateSeriesFilter(series =>
                    series.Seasons.Any(season =>
                        season.Channel.EndsWith(channel, StringComparison.InvariantCultureIgnoreCase)) ||
                    series.SpecialEpisodes.Any(episode =>
                        episode.Channel.EndsWith(channel, StringComparison.InvariantCultureIgnoreCase)))
                : NoFilter;

        private Func<ListItem, bool> SeriesNumberOfSeasonsIs(int numSeasons)
            => this.CreateSeriesFilter(series => series.Seasons.Count == numSeasons);

        private Func<ListItem, bool> SeriesNumberOfSeasonsLessThan(int numSeasons)
            => this.CreateSeriesFilter(series => series.Seasons.Count < numSeasons);

        private Func<ListItem, bool> SeriesNumberOfSeasonsGreaterThan(int numSeasons)
            => this.CreateSeriesFilter(series => series.Seasons.Count > numSeasons);

        private Func<ListItem, bool> SeriesNumberOfSeasonsBetween(int numSeasonsFrom, int numSeasonsTo)
            => this.CreateSeriesFilter(series =>
                numSeasonsFrom <= series.Seasons.Count && series.Seasons.Count <= numSeasonsTo);

        private Func<ListItem, bool> SeriesNumberOfEpisodesIs(int numEpisodes)
            => this.CreateSeriesFilter(series => this.GetNumberOfEpisodes(series) == numEpisodes);

        private Func<ListItem, bool> SeriesNumberOfEpisodesLessThan(int numEpisodes)
            => this.CreateSeriesFilter(series => this.GetNumberOfEpisodes(series) < numEpisodes);

        private Func<ListItem, bool> SeriesNumberOfEpisodesGreaterThan(int numEpisodes)
            => this.CreateSeriesFilter(series => this.GetNumberOfEpisodes(series) > numEpisodes);

        private Func<ListItem, bool> SeriesNumberOfEpisodesBetween(int numEpisodesFrom, int numEpisodesTo)
            => this.CreateSeriesFilter(series =>
            {
                int actualNumEpisodes = this.GetNumberOfEpisodes(series);
                return numEpisodesFrom <= actualNumEpisodes && actualNumEpisodes <= numEpisodesTo;
            });

        private int GetNumberOfEpisodes(Series series)
            => series.Seasons.Sum(season => season.Periods.Sum(period => period.NumberOfEpisodes)) +
                series.SpecialEpisodes.Count;

        private Func<ListItem, bool> SeriesIsMiniseries()
            => this.CreateSeriesFilter(series => series.IsMiniseries);

        private Func<ListItem, bool> SeriesIsAnthology()
            => this.CreateSeriesFilter(series => series.IsAnthology);

        private Func<ListItem, bool> CreateSeriesFilter(Func<Series, bool> predicate)
            => this.CreateFilter(movie => false, predicate);

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
