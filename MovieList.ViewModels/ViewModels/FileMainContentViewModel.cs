using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

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
    public sealed class FileMainContentViewModel : ReactiveObject
    {
        private readonly CompositeDisposable sideViewModelSubscriptions = new CompositeDisposable();
        private readonly CompositeDisposable sideViewModelSecondarySubscriptions = new CompositeDisposable();

        public FileMainContentViewModel(string fileName, ReadOnlyObservableCollection<Kind> kinds)
        {
            this.FileName = fileName;
            this.Kinds = kinds;

            this.List = new ListViewModel(fileName, kinds);

            this.SelectItem = ReactiveCommand.CreateFromTask<ListItem?>(this.OnSelectItemAsync);
            this.Save = ReactiveCommand.Create(() => { });

            this.SideViewModel = this.CreateNewItemViewModel();

            this.List.PreviewSelectItem
                .Select(vm => vm.Item)
                .InvokeCommand(this.SelectItem);

            this.Save.InvokeCommand(this.List.ForceSelectedItem);
        }
        
        public ListViewModel List { get; }

        public string FileName { get; }

        private ReadOnlyObservableCollection<Kind> Kinds { get; }

        [Reactive]
        public ReactiveObject SideViewModel { get; private set; }

        public ReactiveCommand<ListItem?, Unit> SelectItem { get; }
        public ReactiveCommand<Unit, Unit> Save { get; }

        private async Task OnSelectItemAsync(ListItem? item)
        {
            bool canSelect = true;

            if (this.SideViewModel is ISeriesComponentForm seriesComponentForm &&
                (seriesComponentForm.IsFormChanged || seriesComponentForm.Parent.IsFormChanged) ||
                this.SideViewModel is IForm form && form.IsFormChanged)
            {
                canSelect = await Dialog.Confirm.Handle(new ConfirmationModel("CloseForm"));
            }

            if (!canSelect)
            {
                await this.List.ForceSelectedItem.Execute();
                return;
            }

            bool shouldChangeSideViewModel = await this.List.SelectItem.Execute(item);

            if (shouldChangeSideViewModel)
            {
                this.ClearSubscriptions();
                this.SideViewModel = this.CreateSideViewModel(item);
            }
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
                MovieSeriesListItem movieSeriesItem =>
                    this.CreateMovieSeriesForm(movieSeriesItem.MovieSeries),
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

            form.Save
                .Select(m => new MovieListItem(m))
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand<ListItem>(this.List.AddOrUpdate)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Save
                .Discard()
                .InvokeCommand(this.Save)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Close
                .Select<Unit, ListItem?>(_ => null)
                .InvokeCommand(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .InvokeCommand(this.List.RemoveMovie)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .Discard()
                .InvokeCommand(form.Close)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToMovieSeries
                .SubscribeAsync(this.GoToMovieSeriesAsync)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToNext
                .Merge(form.GoToPrevious)
                .SubscribeAsync(this.GoToMovieSeriesEntryAsync)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToMovieSeries
                .Discard()
                .Merge(form.GoToNext.Discard())
                .Merge(form.GoToPrevious.Discard())
                .InvokeCommand(this.List.ForceSelectedItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            return form;
        }

        private SeriesFormViewModel CreateSeriesForm(Series series)
        {
            this.Log().Debug($"Creating a form for series: {series}");

            var form = new SeriesFormViewModel(series, this.Kinds, this.FileName);

            form.Save
                .Select(s => new SeriesListItem(s))
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand<ListItem>(this.List.AddOrUpdate)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Save
                .Discard()
                .InvokeCommand(this.Save)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Close
                .Select<Unit, ListItem?>(_ => null)
                .InvokeCommand(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .InvokeCommand(this.List.RemoveSeries)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .Discard()
                .InvokeCommand(form.Close)
                .DisposeWith(this.sideViewModelSubscriptions);

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

            form.GoToMovieSeries
                .SubscribeAsync(this.GoToMovieSeriesAsync)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToNext
                .Merge(form.GoToPrevious)
                .SubscribeAsync(this.GoToMovieSeriesEntryAsync)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToMovieSeries
                .Discard()
                .Merge(form.GoToNext.Discard())
                .Merge(form.GoToPrevious.Discard())
                .InvokeCommand(this.List.ForceSelectedItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            return form;
        }

        private MiniseriesFormViewModel CreateMiniseriesForm(Series series)
        {
            this.Log().Debug($"Creating a form for miniseries: {series}");

            var form = new MiniseriesFormViewModel(series, this.Kinds, this.FileName);

            form.Save
                .Select(s => new SeriesListItem(s))
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand<ListItem>(this.List.AddOrUpdate)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Save
                .Discard()
                .InvokeCommand(this.Save)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Close
                .Select<Unit, ListItem?>(_ => null)
                .InvokeCommand(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .InvokeCommand(this.List.RemoveSeries)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .Discard()
                .InvokeCommand(form.Close)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.ConvertToSeries
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeAsync(async () => await this.ConvertMiniseriesToSeriesAsync(form))
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToMovieSeries
                .SubscribeAsync(this.GoToMovieSeriesAsync)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToNext
                .Merge(form.GoToPrevious)
                .SubscribeAsync(this.GoToMovieSeriesEntryAsync)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToMovieSeries
                .Discard()
                .Merge(form.GoToNext.Discard())
                .Merge(form.GoToPrevious.Discard())
                .InvokeCommand(this.List.ForceSelectedItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            return form;
        }

        private MovieSeriesFormViewModel CreateMovieSeriesForm(MovieSeries movieSeries)
        {
            this.Log().Debug($"Creating a form for movie series: {movieSeries}");

            var form = new MovieSeriesFormViewModel(movieSeries, this.FileName);
            var detachedEntries = new List<MovieSeriesEntry>();

            form.Save
                .Select(m => new MovieSeriesListItem(m))
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand<ListItem>(this.List.AddOrUpdate)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Save
                .Discard()
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeAsync(async item =>
                {
                    foreach (var entry in detachedEntries)
                    {
                        this.ClearEntryConnection(entry);
                        await this.List.AddOrUpdate.Execute(this.List.EntryToListItem(entry));
                    }

                    detachedEntries.Clear();
                })
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Save
                .Discard()
                .InvokeCommand(this.Save)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Close
                .Select<Unit, ListItem?>(_ => null)
                .InvokeCommand(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .Do(ms => ms.Entries.ForEach(this.ClearEntryConnection))
                .InvokeCommand(this.List.RemoveMovieSeries)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .Discard()
                .InvokeCommand(form.Close)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.SelectEntry
                .SubscribeAsync(this.GoToMovieSeriesEntryAsync)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToMovieSeries
                .SubscribeAsync(this.GoToMovieSeriesAsync)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToNext
                .Merge(form.GoToPrevious)
                .SubscribeAsync(this.GoToMovieSeriesEntryAsync)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.GoToMovieSeries
                .Discard()
                .Merge(form.GoToNext.Discard())
                .Merge(form.GoToPrevious.Discard())
                .InvokeCommand(this.List.ForceSelectedItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.DetachEntry
                .Subscribe(detachedEntries.Add)
                .DisposeWith(this.sideViewModelSubscriptions);

            return form;
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

        private void SubscribeToSeriesComponentEvents<TM, TVm>(
            SeriesComponentFormBase<TM, TVm> form,
            SeriesFormViewModel seriesForm)
            where TM : EntityBase
            where TVm : SeriesComponentFormBase<TM, TVm>
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

        private async Task ConvertMiniseriesToSeriesAsync(MiniseriesFormViewModel miniseriesForm)
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

            if (seriesForm.Seasons.Count == 0)
            {
                await seriesForm.AddSeasonAsync();
            }

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

            this.SideViewModel = seriesForm;
        }

        private async Task GoToMovieSeriesAsync(MovieSeries movieSeries)
        {
            this.ClearSubscriptions();

            this.SideViewModel = this.CreateMovieSeriesForm(movieSeries);

            var listItem = new MovieSeriesListItem(movieSeries);
            await this.SelectItem.Execute(listItem);
            await this.List.ForceSelectedItem.Execute(Unit.Default);
        }

        private async Task GoToMovieSeriesEntryAsync(MovieSeriesEntry entry)
        {
            var listItem = this.List.EntryToListItem(entry);

            this.SideViewModel = this.CreateSideViewModel(listItem);
            await this.SelectItem.Execute(listItem);
            await this.List.ForceSelectedItem.Execute(Unit.Default);
        }

        private void ClearEntryConnection(MovieSeriesEntry entry)
        {
            if (entry.Movie != null)
            {
                entry.Movie.Entry = null;
            } else if (entry.Series != null)
            {
                entry.Series.Entry = null;
            } else if (entry.MovieSeries != null)
            {
                entry.MovieSeries.Entry = null;
            }
        }

        private void ClearSubscriptions()
        {
            this.sideViewModelSubscriptions.Clear();
            this.sideViewModelSecondarySubscriptions.Clear();
        }
    }
}
