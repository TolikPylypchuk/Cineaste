using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
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

using static MovieList.Core.Constants;
using static MovieList.Core.ViewModels.Filters.FilterOperation;
using static MovieList.Core.ViewModels.Filters.FilterType;

using Filter = System.Func<MovieList.Core.ListItems.ListItem, bool>;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class SimpleFilterItemViewModel : FilterItem, IActivatableViewModel
    {
        private readonly SourceList<FilterOperation> availableOperationsSource = new();
        private readonly ReadOnlyObservableCollection<FilterOperation> availableOperations;

        private ReadOnlyObservableCollection<string> tagCategories = null!;
        private ReadOnlyObservableCollection<string> kindNames = null!;
        private readonly ReadOnlyObservableCollection<string> seriesWatchStatuses = null!;
        private readonly ReadOnlyObservableCollection<string> seriesReleaseStatuses = null!;

        private IDisposable inputChangedSubscription = Disposable.Empty;

        public SimpleFilterItemViewModel(
            ReadOnlyObservableCollection<Kind> kinds,
            ReadOnlyObservableCollection<Tag> tags,
            IEnumConverter<SeriesWatchStatus>? seriesWatchStatusConverter = null,
            IEnumConverter<SeriesReleaseStatus>? seriesReleaseStatusConverter = null)
            : base(kinds, tags, seriesWatchStatusConverter, seriesReleaseStatusConverter)
        {
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
                Enum.GetValues<SeriesWatchStatus>().Select(this.SeriesWatchStatusConverter.ToString));

            var seriesReleaseStatuses = new ObservableCollection<string>();

            seriesReleaseStatuses.AddRange(
                Enum.GetValues<SeriesReleaseStatus>().Select(this.SeriesReleaseStatusConverter.ToString));

            this.seriesWatchStatuses = new ReadOnlyObservableCollection<string>(seriesWatchStatuses);
            this.seriesReleaseStatuses = new ReadOnlyObservableCollection<string>(seriesReleaseStatuses);

            this.MakeComposite = ReactiveCommand.Create<FilterComposition, FilterComposition>(c => c);
            this.Delete = ReactiveCommand.Create(() => { });

            this.Delete.Subscribe(this.FilterChangedSubject);

            this.WhenAnyValue(vm => vm.FilterInput)
                .DistinctUntilChanged()
                .Discard()
                .Merge(this.MakeComposite.Discard())
                .Merge(this.WhenAnyValue(vm => vm.IsNegated).Discard())
                .Subscribe(this.FilterChangedSubject);

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
        public FilterType FilterType { get; set; } = ByTitle;

        [Reactive]
        public FilterOperation FilterOperation { get; set; } = Is;

        public FilterInput FilterInput { [ObservableAsProperty] get; } = null!;

        [Reactive]
        public bool IsNegated { get; set; }

        public ReadOnlyObservableCollection<FilterOperation> AvailableOperations
            => this.availableOperations;

        public ReactiveCommand<FilterComposition, FilterComposition> MakeComposite { get; }
        public override ReactiveCommand<Unit, Unit> Delete { get; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public override Filter CreateFilter()
        {
            var coreFilter = this.CreateCoreFilter();
            return this.IsNegated ? item => !coreFilter(item) : coreFilter;
        }

        private Filter CreateCoreFilter()
            => (this.FilterType, this.FilterOperation, this.FilterInput) switch
            {
                (ByTitle, Is, TextFilterInputViewModel input) => this.TitleIs(input.Text),

                (ByTitle, StartsWith, TextFilterInputViewModel input) => this.TitleStartsWith(input.Text),

                (ByTitle, EndsWith, TextFilterInputViewModel input) => this.TitleEndsWith(input.Text),

                (ByYear, Is, NumberFilterInputViewModel input) => this.YearIs(input.Number),

                (ByYear, LessThan, NumberFilterInputViewModel input) => this.YearLessThan(input.Number),

                (ByYear, GreaterThan, NumberFilterInputViewModel input) => this.YearGreaterThan(input.Number),

                (ByYear, Between, RangeFilterInputViewModel input) => this.YearBetween(input.Start, input.End),

                (ByKind, Is, SelectionFilterInputViewModel input) => this.KindIs(input.SelectedItem),

                (ByTags, Include, TagsFilterInputViewModel input) => this.TagsInclude(input.Tags),

                (ByTags, Exclude, TagsFilterInputViewModel input) => this.TagsExclude(input.Tags),

                (ByTags, HaveCategory, SelectionFilterInputViewModel input) =>
                    this.TagsHaveCategory(input.SelectedItem),

                (ByIsStandalone, _, _) => this.IsStandalone(),

                (ByIsMovie, _, _) => this.Movie(),

                (ByMovieIsWatched, _, _) => this.IsMovieWatched(),

                (ByMovieIsReleased, _, _) => this.IsMovieReleased(),

                (ByIsSeries, _, _) => this.Series(),

                (BySeriesWatchStatus, Is, SelectionFilterInputViewModel input) =>
                    this.SeriesWatchStatusIs(input.SelectedItem),

                (BySeriesReleaseStatus, Is, SelectionFilterInputViewModel input) =>
                    this.SeriesReleaseStatusIs(input.SelectedItem),

                (BySeriesChannel, Is, TextFilterInputViewModel input) => this.SeriesChannelIs(input.Text),

                (BySeriesChannel, StartsWith, TextFilterInputViewModel input) =>
                    this.SeriesChannelStartsWith(input.Text),

                (BySeriesChannel, EndsWith, TextFilterInputViewModel input) =>
                    this.SeriesChannelEndsWith(input.Text),

                (BySeriesNumberOfSeasons, Is, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfSeasonsIs(input.Number),

                (BySeriesNumberOfSeasons, LessThan, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfSeasonsLessThan(input.Number),

                (BySeriesNumberOfSeasons, GreaterThan, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfSeasonsGreaterThan(input.Number),

                (BySeriesNumberOfSeasons, Between, RangeFilterInputViewModel input) =>
                    this.SeriesNumberOfSeasonsBetween(input.Start, input.End),

                (BySeriesNumberOfEpisodes, Is, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfEpisodesIs(input.Number),

                (BySeriesNumberOfEpisodes, LessThan, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfEpisodesLessThan(input.Number),

                (BySeriesNumberOfEpisodes, GreaterThan, NumberFilterInputViewModel input) =>
                    this.SeriesNumberOfEpisodesGreaterThan(input.Number),

                (BySeriesNumberOfEpisodes, Between, RangeFilterInputViewModel input) =>
                    this.SeriesNumberOfEpisodesBetween(input.Start, input.End),

                (BySeriesIsMiniseries, _, _) => this.SeriesIsMiniseries(),

                (BySeriesIsAnthology, _, _) => this.SeriesIsAnthology(),

                _ => NoFilter
            };

        private FilterInput CreateFilterInput()
        {
            FilterInput input = (this.FilterType, this.FilterOperation) switch
            {
                (ByTitle, _) =>
                    new TextFilterInputViewModel { Description = "Title" },

                (ByYear, Is or LessThan or GreaterThan) =>
                    new NumberFilterInputViewModel { Description = "Year", Number = DefaultFilterYearValue },

                (ByYear, Between) =>
                    new RangeFilterInputViewModel
                    {
                        Description = "Year",
                        Start = DefaultFilterYearValue,
                        End = DefaultFilterYearValue
                    },

                (ByKind, Is) =>
                    new SelectionFilterInputViewModel(this.kindNames) { Description = "Kind" },

                (ByTags, Include or Exclude) =>
                    new TagsFilterInputViewModel(this.Tags) { Description = "Tags" },

                (ByTags, HaveCategory) =>
                    new SelectionFilterInputViewModel(this.tagCategories) { Description = "TagCategory" },

                (BySeriesWatchStatus, _) =>
                    new SelectionFilterInputViewModel(this.seriesWatchStatuses)
                    { Description = "SeriesWatchStatus" },

                (BySeriesReleaseStatus, _) =>
                    new SelectionFilterInputViewModel(this.seriesReleaseStatuses)
                    { Description = "SeriesReleaseStatus" },

                (BySeriesChannel, _) =>
                    new TextFilterInputViewModel { Description = "SeriesChannel" },

                (BySeriesNumberOfSeasons, Is or LessThan or GreaterThan) =>
                    new NumberFilterInputViewModel
                    {
                        Description = "SeriesNumberOfSeasons",
                        Number = DefaultFilterNumberOfSeasonsValue
                    },

                (BySeriesNumberOfSeasons, Between) =>
                    new RangeFilterInputViewModel
                    {
                        Description = "SeriesNumberOfSeasons",
                        Start = DefaultFilterNumberOfSeasonsValue,
                        End = DefaultFilterNumberOfSeasonsValue
                    },

                (BySeriesNumberOfEpisodes, Is or LessThan or GreaterThan) =>
                    new NumberFilterInputViewModel
                    {
                        Description = "SeriesNumberOfEpisodes",
                        Number = DefaultFilterNumberOfSeasonsValue
                    },

                (BySeriesNumberOfEpisodes, Between) =>
                    new RangeFilterInputViewModel
                    {
                        Description = "SeriesNumberOfEpisodes",
                        Start = DefaultFilterNumberOfSeasonsValue,
                        End = DefaultFilterNumberOfSeasonsValue
                    },

                _ => new NoFilterInputViewModel()
            };

            this.inputChangedSubscription.Dispose();
            this.inputChangedSubscription = input.InputChanged.Subscribe(this.FilterChangedSubject);

            return input;
        }

        private Filter TitleIs(string? title)
            => !String.IsNullOrEmpty(title)
                ? this.TitleFilter(t => t.Contains(title, StringComparison.InvariantCultureIgnoreCase))
                : NoFilter;

        private Filter TitleStartsWith(string? title)
            => !String.IsNullOrEmpty(title)
                ? this.TitleFilter(t => t.StartsWith(title, StringComparison.InvariantCultureIgnoreCase))
                : NoFilter;

        private Filter TitleEndsWith(string? title)
            => !String.IsNullOrEmpty(title)
                ? this.TitleFilter(t => t.EndsWith(title, StringComparison.InvariantCultureIgnoreCase))
                : NoFilter;

        private Filter TitleFilter(Func<string, bool> predicate)
            => this.CreateFilter(
                movie => movie.Titles.Any(t => predicate(t.Name)),
                series => series.Titles.Any(t => predicate(t.Name)));

        private Filter YearIs(int year)
            => this.CreateFilter(movie => movie.Year == year, series => this.SeriesYearIs(series, year));

        private bool SeriesYearIs(Series series, int year)
            => series.Seasons.Any(season => season.Periods
                .Any(period => period.StartYear <= year && period.EndYear >= year)) ||
                series.SpecialEpisodes.Any(episode => episode.Year == year);

        private Filter YearLessThan(int year)
            => this.CreateFilter(movie => movie.Year < year, series => series.StartYear < year);

        private Filter YearGreaterThan(int year)
            => this.CreateFilter(movie => movie.Year > year, series => series.EndYear > year);

        private Filter YearBetween(int startYear, int endYear)
            => startYear <= endYear
                ? this.CreateFilter(
                    movie => startYear <= movie.Year && movie.Year <= endYear,
                    series => this.SeriesYearBetween(series, startYear, endYear))
                : NoFilter;

        private bool SeriesYearBetween(Series series, int startYear, int endYear)
            => series.Seasons.Any(season => season.Periods
                .Any(period => period.StartYear <= endYear && period.EndYear >= startYear)) ||
                series.SpecialEpisodes.Any(episode => startYear <= episode.Year && episode.Year <= endYear);

        private Filter KindIs(string? kindName)
            => !String.IsNullOrEmpty(kindName)
                ? this.CreateFilter(movie => movie.Kind.Name == kindName, series => series.Kind.Name == kindName)
                : NoFilter;

        private Filter TagsInclude(IEnumerable<TagItemViewModel> tagItems)
            => this.CreateTagFilter(
                tagItems,
                (movie, tags) => tags.All(tag => movie.Tags.Contains(tag)),
                (series, tags) => tags.All(tag => series.Tags.Contains(tag)));

        private Filter TagsExclude(IEnumerable<TagItemViewModel> tagItems)
            => this.CreateTagFilter(
                tagItems,
                (movie, tags) => tags.All(tag => !movie.Tags.Contains(tag)),
                (series, tags) => tags.All(tag => !series.Tags.Contains(tag)));

        private Filter CreateTagFilter(
            IEnumerable<TagItemViewModel> tagItems,
            Func<Movie, List<Tag>, bool> moviePredicate,
            Func<Series, List<Tag>, bool> seriesPredicate)
        {
            var tags = tagItems.Select(item => item.Tag).ToList();
            return !tags.IsEmpty()
                ? this.CreateFilter(movie => moviePredicate(movie, tags), series => seriesPredicate(series, tags))
                : NoFilter;
        }

        private Filter TagsHaveCategory(string? category)
            => !String.IsNullOrEmpty(category)
                ? this.CreateFilter(
                    movie => movie.Tags.Any(tag => tag.Category == category),
                    series => series.Tags.Any(tag => tag.Category == category))
                : NoFilter;

        private Filter IsStandalone()
            => item => item switch
            {
                MovieListItem movieItem => movieItem.Movie.Entry == null,
                SeriesListItem seriesItem => seriesItem.Series.Entry == null,
                _ => false
            };

        private Filter Movie()
            => this.CreateMovieFilter(movie => true);

        private Filter IsMovieWatched()
            => this.CreateMovieFilter(movie => movie.IsWatched);

        private Filter IsMovieReleased()
            => this.CreateMovieFilter(movie => movie.IsReleased);

        private Filter CreateMovieFilter(Func<Movie, bool> predicate)
            => this.CreateFilter(predicate, series => false);

        private Filter Series()
            => this.CreateSeriesFilter(series => true);

        private Filter SeriesWatchStatusIs(string? status)
            => !String.IsNullOrEmpty(status)
                ? this.CreateSeriesFilter(
                    series => this.SeriesWatchStatusConverter.ToString(series.WatchStatus) == status)
                : NoFilter;

        private Filter SeriesReleaseStatusIs(string? status)
            => !String.IsNullOrEmpty(status)
                ? this.CreateSeriesFilter(
                    series => this.SeriesReleaseStatusConverter.ToString(series.ReleaseStatus) == status)
                : NoFilter;

        private Filter SeriesChannelIs(string? channel)
            => !String.IsNullOrEmpty(channel)
                ? this.CreateSeriesFilter(series =>
                    series.Seasons.Any(season =>
                        season.Channel.Equals(channel, StringComparison.InvariantCultureIgnoreCase)) ||
                    series.SpecialEpisodes.Any(episode =>
                        episode.Channel.Equals(channel, StringComparison.InvariantCultureIgnoreCase)))
                : NoFilter;

        private Filter SeriesChannelStartsWith(string? channel)
            => !String.IsNullOrEmpty(channel)
                ? this.CreateSeriesFilter(series =>
                    series.Seasons.Any(season =>
                        season.Channel.StartsWith(channel, StringComparison.InvariantCultureIgnoreCase)) ||
                    series.SpecialEpisodes.Any(episode =>
                        episode.Channel.StartsWith(channel, StringComparison.InvariantCultureIgnoreCase)))
                : NoFilter;

        private Filter SeriesChannelEndsWith(string? channel)
            => !String.IsNullOrEmpty(channel)
                ? this.CreateSeriesFilter(series =>
                    series.Seasons.Any(season =>
                        season.Channel.EndsWith(channel, StringComparison.InvariantCultureIgnoreCase)) ||
                    series.SpecialEpisodes.Any(episode =>
                        episode.Channel.EndsWith(channel, StringComparison.InvariantCultureIgnoreCase)))
                : NoFilter;

        private Filter SeriesNumberOfSeasonsIs(int numSeasons)
            => this.CreateSeriesFilter(series => series.Seasons.Count == numSeasons);

        private Filter SeriesNumberOfSeasonsLessThan(int numSeasons)
            => this.CreateSeriesFilter(series => series.Seasons.Count < numSeasons);

        private Filter SeriesNumberOfSeasonsGreaterThan(int numSeasons)
            => this.CreateSeriesFilter(series => series.Seasons.Count > numSeasons);

        private Filter SeriesNumberOfSeasonsBetween(int numSeasonsFrom, int numSeasonsTo)
            => this.CreateSeriesFilter(series =>
                numSeasonsFrom <= series.Seasons.Count && series.Seasons.Count <= numSeasonsTo);

        private Filter SeriesNumberOfEpisodesIs(int numEpisodes)
            => this.CreateSeriesFilter(series => this.GetNumberOfEpisodes(series) == numEpisodes);

        private Filter SeriesNumberOfEpisodesLessThan(int numEpisodes)
            => this.CreateSeriesFilter(series => this.GetNumberOfEpisodes(series) < numEpisodes);

        private Filter SeriesNumberOfEpisodesGreaterThan(int numEpisodes)
            => this.CreateSeriesFilter(series => this.GetNumberOfEpisodes(series) > numEpisodes);

        private Filter SeriesNumberOfEpisodesBetween(int numEpisodesFrom, int numEpisodesTo)
            => this.CreateSeriesFilter(series =>
            {
                int actualNumEpisodes = this.GetNumberOfEpisodes(series);
                return numEpisodesFrom <= actualNumEpisodes && actualNumEpisodes <= numEpisodesTo;
            });

        private int GetNumberOfEpisodes(Series series)
            => series.Seasons.Sum(season => season.Periods.Sum(period => period.NumberOfEpisodes)) +
                series.SpecialEpisodes.Count;

        private Filter SeriesIsMiniseries()
            => this.CreateSeriesFilter(series => series.IsMiniseries);

        private Filter SeriesIsAnthology()
            => this.CreateSeriesFilter(series => series.IsAnthology);

        private Filter CreateSeriesFilter(Func<Series, bool> predicate)
            => this.CreateFilter(movie => false, predicate);

        private Filter CreateFilter(Func<Movie, bool> moviePredicate, Func<Series, bool> seriesPredicate)
        {
            Filter filter = null!;
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
