using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using DynamicData;

using MovieList.Data;
using MovieList.Data.Models;
using MovieList.DialogModels;
using MovieList.ListItems;
using MovieList.ViewModels.Forms;
using MovieList.ViewModels.Forms.Base;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels
{
    public sealed class FileMainContentViewModel : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable sideViewModelSubscriptions = new CompositeDisposable();
        private readonly CompositeDisposable sideViewModelSecondarySubscriptions = new CompositeDisposable();
        private readonly BehaviorSubject<bool> areUnsavedChangesPresentSubject = new BehaviorSubject<bool>(false);

        private readonly SourceList<FranchiseEntry> franchiseAddableItemsSource =
            new SourceList<FranchiseEntry>();

        private readonly ReadOnlyObservableCollection<FranchiseEntry> franchiseAddableItems;

        public FileMainContentViewModel(string fileName, ReadOnlyObservableCollection<Kind> kinds)
        {
            this.FileName = fileName;
            this.Kinds = kinds;

            this.List = new ListViewModel(fileName, kinds);

            this.franchiseAddableItemsSource.Connect()
                .Bind(out this.franchiseAddableItems)
                .Subscribe();

            this.WhenAnyValue(vm => vm.List.MovieList)
                .WhereNotNull()
                .Select(list => list.ToEntries())
                .Select(entries => entries.Where(entry =>
                        entry.Movie?.Entry == null && entry.Series?.Entry == null && entry.Franchise?.Entry == null))
                .Subscribe(this.franchiseAddableItemsSource.AddRange);

            this.areUnsavedChangesPresentSubject.ToPropertyEx(this, vm => vm.AreUnsavedChangesPresent);

            this.SelectItem = ReactiveCommand.CreateFromObservable<ListItem?, Unit>(
                this.OnSelectItem, this.WhenAnyValue(vm => vm.CanSelectItem));

            this.TunnelSave = ReactiveCommand.Create(() => { });
            this.BubbleSave = ReactiveCommand.Create(() => { });

            this.SideViewModel = this.CreateNewItemViewModel();

            this.List.PreviewSelectItem
                .Select(vm => vm.Item)
                .InvokeCommand(this.SelectItem);

            this.BubbleSave.InvokeCommand(this.List.ForceSelectedItem);

            this.WhenAnyValue(vm => vm.SideViewModel)
                .OfType<NewItemViewModel>()
                .Select(_ => false)
                .Subscribe(this.areUnsavedChangesPresentSubject);
        }
        
        public ListViewModel List { get; }

        public string FileName { get; }

        public ReadOnlyObservableCollection<FranchiseEntry> FranchiseAddableItems
            => this.franchiseAddableItems;

        public ReadOnlyObservableCollection<Kind> Kinds { get; }

        [Reactive]
        public ReactiveObject SideViewModel { get; private set; }

        public bool AreUnsavedChangesPresent { [ObservableAsProperty] get; }

        [Reactive]
        public bool CanSelectItem { get; private set; } = true;

        public ReactiveCommand<ListItem?, Unit> SelectItem { get; }
        public ReactiveCommand<Unit, Unit> TunnelSave { get; }
        public ReactiveCommand<Unit, Unit> BubbleSave { get; }

        public void Dispose()
            => this.CanSelectItem = false;

        private IObservable<Unit> OnSelectItem(ListItem? item)
        {
            var canSelectObservable = this.AreUnsavedChangesPresent
                ? Dialog.Confirm.Handle(new ConfirmationModel("CloseForm"))
                : Observable.Return(true);

            return canSelectObservable
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(canSelect => !canSelect
                    ? this.List.ForceSelectedItem.Execute()
                    : this.List.SelectItem.Execute(item)
                        .DoIfTrue(() =>
                        {
                            this.ClearSubscriptions();
                            this.SideViewModel = this.CreateSideViewModel(item);
                        })
                        .Discard());
        }

        private ReactiveObject CreateSideViewModel(ListItem? item)
            => item switch
            {
                null =>
                    this.CreateNewItemViewModel(),
                MovieListItem movieItem =>
                    this.CreateMovieForm(movieItem.Movie),
                SeriesListItem seriesItem when !seriesItem.Series.IsMiniseries =>
                    this.CreateSeriesForm(seriesItem.Series),
                SeriesListItem seriesItem when seriesItem.Series.IsMiniseries =>
                    this.CreateMiniseriesForm(seriesItem.Series),
                FranchiseListItem franchiseItem =>
                    this.CreateFranchiseForm(franchiseItem.Franchise),
                _ =>
                    throw new NotSupportedException("List item type not supported")
            };

        private NewItemViewModel CreateNewItemViewModel()
        {
            var viewModel = new NewItemViewModel();

            this.ClearSubscriptions();

            viewModel.AddNewMovie
                .Select(_ => new Movie
                {
                    Titles = new List<Title>
                    {
                        new Title { Priority = 1, IsOriginal = false },
                        new Title { Priority = 1, IsOriginal = true }
                    },
                    Year = MovieDefaultYear,
                    Kind = this.Kinds.First()
                })
                .Select(movie => new MovieListItem(movie))
                .InvokeCommand<ListItem>(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            viewModel.AddNewSeries
                .Select(_ => new Series
                {
                    Titles = new List<Title>
                    {
                        new Title { Priority = 1, IsOriginal = false},
                        new Title { Priority = 1, IsOriginal = true}
                    },
                    Kind = this.Kinds.First()
                })
                .Select(series => new SeriesListItem(series))
                .InvokeCommand<ListItem>(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            return viewModel;
        }

        private MovieFormViewModel CreateMovieForm(Movie movie)
        {
            this.Log().Debug($"Creating a form for movie: {movie}");

            var form = new MovieFormViewModel(movie, this.Kinds, this.FileName);

            this.SubscribeToCommonCommands(
                form, this.List.RemoveMovie, m => new MovieListItem(m), e => e.Movie = form.Movie);

            return form;
        }

        private SeriesFormViewModel CreateSeriesForm(Series series)
        {
            this.Log().Debug($"Creating a form for series: {series}");

            var form = new SeriesFormViewModel(series, this.Kinds, this.FileName);

            this.SubscribeToCommonCommands(
                form, this.List.RemoveSeries, s => new SeriesListItem(s), e => e.Series = form.Series);

            form.SelectComponent
                .OfType<SeasonFormViewModel>()
                .Subscribe(seasonForm => this.OpenSeasonForm(seasonForm, form))
                .DisposeWith(this.sideViewModelSubscriptions);

            form.SelectComponent
                .OfType<SpecialEpisodeFormViewModel>()
                .Subscribe(episodeForm => this.OpenSpecialEpisodeForm(episodeForm, form))
                .DisposeWith(this.sideViewModelSubscriptions);

            form.ConvertToMiniseries
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(() => this.ConvertSeriesToMiniseries(form))
                .DisposeWith(this.sideViewModelSubscriptions);

            return form;
        }

        private MiniseriesFormViewModel CreateMiniseriesForm(Series series)
        {
            this.Log().Debug($"Creating a form for miniseries: {series}");

            var form = new MiniseriesFormViewModel(series, this.Kinds, this.FileName);

            this.SubscribeToCommonCommands(
                form, this.List.RemoveSeries, s => new SeriesListItem(s), e => e.Series = form.Series);

            form.ConvertToSeries
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeAsync(() => this.ConvertMiniseriesToSeries(form))
                .DisposeWith(this.sideViewModelSubscriptions);

            return form;
        }

        private FranchiseFormViewModel CreateFranchiseForm(Franchise franchise)
        {
            this.Log().Debug($"Creating a form for franchise: {franchise}");

            var form = new FranchiseFormViewModel(franchise, this.FileName, this.FranchiseAddableItems);
            var attachedEntries = new List<FranchiseEntry>();
            var detachedEntries = new List<FranchiseEntry>();

            this.SubscribeToCommonCommands(
                form,
                this.List.RemoveFranchise,
                f => new FranchiseListItem(f),
                e => e.Franchise = form.Franchise);

            form.Save
                .Discard()
                .ObserveOn(RxApp.MainThreadScheduler)
                .DoAsync(() => this.AttachEntries(attachedEntries))
                .DoAsync(() => this.DetachEntries(detachedEntries))
                .Subscribe(() =>
                {
                    attachedEntries.Clear();
                    detachedEntries.Clear();
                })
                .DisposeWith(this.sideViewModelSubscriptions);

            form.SelectEntry
                .SubscribeAsync(this.GoToFranchiseEntry)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.DetachEntry
                .Subscribe(detachedEntries.Add)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.DetachEntry
                .Subscribe(entry => this.RemoveSameEntry(attachedEntries, entry, attachedEntries.RemoveMany))
                .DisposeWith(this.sideViewModelSubscriptions);

            form.AddMovie
                .Select(displayNumber => this.CreateMovieForFranchise(form.Franchise, displayNumber))
                .Select(movie => new MovieListItem(movie))
                .InvokeCommand<ListItem>(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.AddSeries
                .Select(displayNumber => this.CreateSeriesForFranchise(form.Franchise, displayNumber))
                .Select(series => new SeriesListItem(series))
                .InvokeCommand<ListItem>(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.AddFranchise
                .Select(displayNumber => this.CreateFranchiseForFranchise(form.Franchise, displayNumber))
                .Select(f => new FranchiseListItem(f))
                .InvokeCommand<ListItem>(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.AddExistingItem
                .Subscribe(attachedEntries.Add)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.AddExistingItem
                .Subscribe(entry => this.RemoveSameEntry(detachedEntries, entry, detachedEntries.RemoveMany))
                .DisposeWith(this.sideViewModelSubscriptions);

            return form;
        }

        private IObservable<Unit> AttachEntries(List<FranchiseEntry> entries)
            => entries.Count == 0
                ? Observable.Return(Unit.Default)
                : entries
                    .Select(entry => this.List.AddOrUpdate
                        .Execute(this.List.EntryToListItem(entry))
                        .Do(_ => this.RemoveSameEntry(
                            this.franchiseAddableItemsSource.Items,
                            entry,
                            this.franchiseAddableItemsSource.RemoveMany)))
                    .ForkJoin()
                    .Discard();

        private IObservable<Unit> DetachEntries(List<FranchiseEntry> entries)
            => entries.Count == 0
                ? Observable.Return(Unit.Default)
                : entries
                    .Do(this.ClearEntryConnection)
                    .Select(entry => this.List.AddOrUpdate
                        .Execute(this.List.EntryToListItem(entry))
                        .Do(_ => this.franchiseAddableItemsSource.Add(entry)))
                    .ForkJoin()
                    .Discard();

        private void SubscribeToCommonCommands<TModel, TForm>(
            FranchiseEntryFormBase<TModel, TForm> form,
            ReactiveCommand<TModel, Unit> removeFromList,
            Func<TModel, ListItem> listItemSelector,
            Action<FranchiseEntry> entryRelationSetter)
            where TModel : class
            where TForm : FranchiseEntryFormBase<TModel, TForm>
        {
            this.TunnelSave
                .InvokeCommand(form.Save)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Save
                .Select(listItemSelector)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand<ListItem>(this.List.AddOrUpdate)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Save
                .Discard()
                .InvokeCommand(this.BubbleSave)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Close
                .Select<Unit, ListItem?>(_ => null)
                .InvokeCommand(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .InvokeCommand(removeFromList)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .Discard()
                .InvokeCommand(form.Close)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToFranchise
                .SubscribeAsync(this.GoToFranchise)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToNext
                .Merge(form.GoToPrevious)
                .SubscribeAsync(this.GoToFranchiseEntry)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.CreateFranchise
                .Select(_ =>
                {
                    var franchise = new Franchise();
                    var entry = new FranchiseEntry
                    {
                        SequenceNumber = 1,
                        DisplayNumber = 1,
                        ParentFranchise = franchise
                    };

                    entryRelationSetter(entry);
                    franchise.Entries.Add(entry);

                    return franchise;
                })
                .Select(f => new FranchiseListItem(f))
                .InvokeCommand<ListItem>(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToFranchise
                .Discard()
                .Merge(form.GoToNext.Discard())
                .Merge(form.GoToPrevious.Discard())
                .Merge(form.CreateFranchise)
                .InvokeCommand(this.List.ForceSelectedItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Cancel.CanExecute
                .Subscribe(this.areUnsavedChangesPresentSubject)
                .DisposeWith(this.sideViewModelSubscriptions);
        }

        private void OpenSeasonForm(SeasonFormViewModel form, SeriesFormViewModel seriesForm)
        {
            this.Log().Debug($"Opening a form for season: {form.Season}");

            this.SubscribeToSeriesComponentEvents(form, seriesForm);

            this.SideViewModel = form;
        }

        private void OpenSpecialEpisodeForm(SpecialEpisodeFormViewModel form, SeriesFormViewModel seriesForm)
        {
            this.Log().Debug($"Opening a form for special episode: {form.SpecialEpisode}");

            this.SubscribeToSeriesComponentEvents(form, seriesForm);

            this.SideViewModel = form;
        }

        private void SubscribeToSeriesComponentEvents<TModel, TForm>(
            SeriesComponentFormBase<TModel, TForm> form,
            SeriesFormViewModel seriesForm)
            where TModel : EntityBase
            where TForm : SeriesComponentFormBase<TModel, TForm>
        {
            form.Close
                .Select<Unit, ListItem?>(_ => null)
                .InvokeCommand(this.SelectItem)
                .DisposeWith(this.sideViewModelSecondarySubscriptions);

            form.Delete
                .WhereNotNull()
                .Subscribe(_ => this.SideViewModel = seriesForm)
                .DisposeWith(this.sideViewModelSecondarySubscriptions);

            form.GoToSeries
                .Subscribe(parentForm => this.SideViewModel = parentForm)
                .DisposeWith(this.sideViewModelSecondarySubscriptions);

            form.GoToSeries
                .Subscribe(_ => this.sideViewModelSecondarySubscriptions.Clear())
                .DisposeWith(this.sideViewModelSecondarySubscriptions);

            form.Cancel.CanExecute
                .CombineLatest(form.Parent.Cancel.CanExecute, (a, b) => a || b)
                .Subscribe(this.areUnsavedChangesPresentSubject)
                .DisposeWith(this.sideViewModelSecondarySubscriptions);
        }

        private void ConvertSeriesToMiniseries(SeriesFormViewModel seriesForm)
        {
            this.Log().Debug($"Converting a series to miniseries: {seriesForm.Series}");

            this.ClearSubscriptions();

            var miniseriesForm = this.CreateMiniseriesForm(seriesForm.Series);

            miniseriesForm.Kind = seriesForm.Kind;
            miniseriesForm.IsAnthology = seriesForm.IsAnthology;
            miniseriesForm.WatchStatus = seriesForm.WatchStatus;
            miniseriesForm.ReleaseStatus = seriesForm.ReleaseStatus;
            miniseriesForm.ImdbLink = seriesForm.ImdbLink;
            miniseriesForm.PosterUrl = seriesForm.PosterUrl;

            if (seriesForm.Seasons.Count == 1)
            {
                var seasonForm = seriesForm.Seasons[0];

                miniseriesForm.Channel = seasonForm.Channel;

                var periodForm = seasonForm.Periods[0];

                miniseriesForm.PeriodForm.StartMonth = periodForm.StartMonth;
                miniseriesForm.PeriodForm.StartYear = periodForm.StartYear;
                miniseriesForm.PeriodForm.EndMonth = periodForm.EndMonth;
                miniseriesForm.PeriodForm.EndYear = periodForm.EndYear;
                miniseriesForm.PeriodForm.NumberOfEpisodes = periodForm.NumberOfEpisodes;
                miniseriesForm.PeriodForm.IsSingleDayRelease = periodForm.IsSingleDayRelease;
                miniseriesForm.PeriodForm.PosterUrl = periodForm.PosterUrl;
            }

            this.SideViewModel = miniseriesForm;
        }

        private IObservable<Unit> ConvertMiniseriesToSeries(MiniseriesFormViewModel miniseriesForm)
        {
            this.Log().Debug($"Converting a miniseries to series: {miniseriesForm.Series}");

            this.ClearSubscriptions();

            var seriesForm = this.CreateSeriesForm(miniseriesForm.Series);

            seriesForm.Kind = miniseriesForm.Kind;
            seriesForm.IsAnthology = miniseriesForm.IsAnthology;
            seriesForm.WatchStatus = miniseriesForm.WatchStatus;
            seriesForm.ReleaseStatus = miniseriesForm.ReleaseStatus;
            seriesForm.ImdbLink = miniseriesForm.ImdbLink;
            seriesForm.PosterUrl = miniseriesForm.PosterUrl;

            var seasonObservable = seriesForm.Seasons.Count == 0
                ? seriesForm.AddSeason.Execute()
                : Observable.Return(Unit.Default);

            return seasonObservable
                .Do(() => this.ConvertMiniseriesToSeason(miniseriesForm, seriesForm))
                .Do(() => this.SideViewModel = seriesForm);
        }

        private void ConvertMiniseriesToSeason(MiniseriesFormViewModel miniseriesForm, SeriesFormViewModel seriesForm)
        {
            var seasonForm = seriesForm.Seasons[0];

            seasonForm.Channel = miniseriesForm.Channel;

            var periodForm = seasonForm.Periods[0];

            periodForm.StartMonth = miniseriesForm.PeriodForm.StartMonth;
            periodForm.StartYear = miniseriesForm.PeriodForm.StartYear;
            periodForm.EndMonth = miniseriesForm.PeriodForm.EndMonth;
            periodForm.EndYear = miniseriesForm.PeriodForm.EndYear;
            periodForm.NumberOfEpisodes = miniseriesForm.PeriodForm.NumberOfEpisodes;
            periodForm.IsSingleDayRelease = miniseriesForm.PeriodForm.IsSingleDayRelease;
            periodForm.PosterUrl = miniseriesForm.PeriodForm.PosterUrl;
        }

        private IObservable<Unit> GoToFranchise(Franchise franchise)
        {
            this.ClearSubscriptions();

            this.SideViewModel = this.CreateFranchiseForm(franchise);

            return this.SelectItem.Execute(new FranchiseListItem(franchise))
                .DoAsync(this.List.ForceSelectedItem.Execute);
        }

        private IObservable<Unit> GoToFranchiseEntry(FranchiseEntry entry)
        {
            var listItem = this.List.EntryToListItem(entry);

            this.SideViewModel = this.CreateSideViewModel(listItem);

            return this.SelectItem.Execute(listItem)
                .DoAsync(this.List.ForceSelectedItem.Execute);
        }

        private Movie CreateMovieForFranchise(Franchise parentFranchise, int displayNumber)
        {
            var entry = this.CreateEntryForFranchise(parentFranchise, displayNumber);

            var movie = new Movie
            {
                Titles = new List<Title>
                {
                    new Title { Priority = 1, IsOriginal = false },
                    new Title { Priority = 1, IsOriginal = true }
                },
                Year = MovieDefaultYear,
                Kind = this.Kinds.First(),
                Entry = entry
            };

            entry.Movie = movie;

            return movie;
        }

        private Series CreateSeriesForFranchise(Franchise parentFranchise, int displayNumber)
        {
            var entry = this.CreateEntryForFranchise(parentFranchise, displayNumber);

            var series = new Series
            {
                Titles = new List<Title>
                {
                    new Title { Priority = 1, IsOriginal = false},
                    new Title { Priority = 1, IsOriginal = true}
                },
                Kind = this.Kinds.First(),
                Entry = entry
            };

            entry.Series = series;

            return series;
        }

        private Franchise CreateFranchiseForFranchise(Franchise parentFranchise, int displayNumber)
        {
            var entry = this.CreateEntryForFranchise(parentFranchise, displayNumber);

            var franchise = new Franchise { Entry = entry };

            entry.Franchise = franchise;

            return franchise;
        }

        private FranchiseEntry CreateEntryForFranchise(Franchise parentFranchise, int displayNumber)
            => new FranchiseEntry
            {
                ParentFranchise = parentFranchise,
                SequenceNumber = parentFranchise.Entries.Count == 0
                    ? 1
                    : parentFranchise.Entries.Select(e => e.SequenceNumber).Max() + 1,
                DisplayNumber = parentFranchise.Entries.Count == 0
                    ? displayNumber
                    : (parentFranchise.Entries.Select(e => e.DisplayNumber).Max() ?? (displayNumber - 1)) + 1
            };

        private void ClearEntryConnection(FranchiseEntry entry)
        {
            if (entry.Movie != null)
            {
                entry.Movie.Entry = null;
            } else if (entry.Series != null)
            {
                entry.Series.Entry = null;
            } else if (entry.Franchise != null)
            {
                entry.Franchise.Entry = null;
            }
        }

        private void ClearSubscriptions()
        {
            this.sideViewModelSubscriptions.Clear();
            this.sideViewModelSecondarySubscriptions.Clear();
        }

        private void RemoveSameEntry(
            IEnumerable<FranchiseEntry> entries,
            FranchiseEntry entry,
            Action<IEnumerable<FranchiseEntry>> remove)
            => remove(entries.Where(e =>
                (e.Movie != null && e.Movie == entry.Movie) ||
                (e.Series != null && e.Series == entry.Series) ||
                (e.Franchise != null && e.Franchise == entry.Franchise)));
    }
}
