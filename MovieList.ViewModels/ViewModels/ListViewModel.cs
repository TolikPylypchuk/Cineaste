using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

using DynamicData;

using MovieList.Comparers;
using MovieList.Data;
using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.DialogModels;
using MovieList.ListItems;
using MovieList.ViewModels.Forms;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels
{
    public sealed class ListViewModel : ReactiveObject, IEnableLogger
    {
        private readonly ISourceCache<ListItem, string> source;
        private readonly ReadOnlyObservableCollection<ListItemViewModel> items;
        private readonly Subject<Unit> cancelSelection = new Subject<Unit>();
        private readonly CompositeDisposable sideViewModelSubscriptions = new CompositeDisposable();
        private readonly CompositeDisposable sideViewModelSecondarySubscriptions = new CompositeDisposable();

        public ListViewModel(
            string fileName,
            ReadOnlyObservableCollection<Kind> kinds,
            IListService? listService = null)
        {
            this.FileName = fileName;
            this.Kinds = kinds;

            listService ??= Locator.Current.GetService<IListService>(fileName);

            this.source = new SourceCache<ListItem, string>(item => item.Id);

            this.source.PopulateFrom(
                Observable.FromAsync(() => Task.Run(() => listService.GetListAsync(kinds)))
                    .Select(list => list.ToListItems())
                    .Do(items => this.Log().Debug($"Loaded the list of {items.Count} items")));

            this.source.Connect()
                .Filter(item => !String.IsNullOrEmpty(item.Title))
                .AutoRefresh(item => item.DisplayNumber)
                .AutoRefresh(item => item.Title)
                .AutoRefresh(item => item.OriginalTitle)
                .AutoRefresh(item => item.Year)
                .Transform(item => new ListItemViewModel(item))
                .Sort(new PropertyComparer<ListItemViewModel, ListItem>(vm => vm.Item, ListItemTitleComparer.Instance))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.items)
                .Subscribe();

            this.SelectItem = ReactiveCommand.CreateFromTask<ListItemViewModel?, bool>(this.OnSelectItemAsync);
            this.Save = ReactiveCommand.Create(() => { });

            this.SideViewModel = this.CreateNewItemViewModel();
        }

        public string FileName { get; }

        private ReadOnlyObservableCollection<Kind> Kinds { get; }

        public ReadOnlyObservableCollection<ListItemViewModel> Items
            => this.items;

        [Reactive]
        public ListItemViewModel? SelectedItem { get; set; }

        [Reactive]
        public ReactiveObject SideViewModel { get; private set; }

        public IObservable<Unit> CancelSelection
            => this.cancelSelection.AsObservable();

        public ReactiveCommand<ListItemViewModel?, bool> SelectItem { get; }
        public ReactiveCommand<Unit, Unit> Save { get; }

        private async Task<bool> OnSelectItemAsync(ListItemViewModel? vm)
        {
            bool canSelect = true;

            if (this.IsMovieFormChanged() || this.IsSeriesFormChanged() ||
                this.IsSeasonFormChanged() || this.IsSpecialEpisodeFormChanged())
            {
                canSelect = await Dialog.Confirm.Handle(new ConfirmationModel("CloseForm"));
            }

            if (!canSelect)
            {
                this.cancelSelection.OnNext(Unit.Default);
                return false;
            }

            bool isSame = this.SelectedItem?.Item.Id == vm?.Item.Id;

            this.SelectedItem = vm;

            if (!isSame)
            {
                this.sideViewModelSubscriptions.Clear();
                this.sideViewModelSecondarySubscriptions.Clear();

                this.SideViewModel = vm?.Item switch
                {
                    null => this.CreateNewItemViewModel(),
                    MovieListItem movieItem => this.CreateMovieForm(movieItem.Movie),
                    SeriesListItem seriesItem => this.CreateSeriesForm(seriesItem.Series),
                    MovieSeriesListItem _ => this.CreateNewItemViewModel(),
                    _ => throw new NotSupportedException("List item type not supported")
                };
            }

            return true;
        }

        private bool IsMovieFormChanged()
            => this.SideViewModel is MovieFormViewModel movieForm && movieForm.IsFormChanged;

        private bool IsSeriesFormChanged()
            => this.SideViewModel is SeriesFormViewModel seriesForm && seriesForm.IsFormChanged;

        private bool IsSeasonFormChanged()
            => this.SideViewModel is SeasonFormViewModel seasonForm &&
               (seasonForm.IsFormChanged || seasonForm.Parent.IsFormChanged);

        private bool IsSpecialEpisodeFormChanged()
            => this.SideViewModel is SpecialEpisodeFormViewModel episodeForm &&
               (episodeForm.IsFormChanged || episodeForm.Parent.IsFormChanged);

        private ReactiveObject CreateNewItemViewModel()
        {
            var viewModel = new NewItemViewModel();

            this.sideViewModelSubscriptions.Clear();
            this.sideViewModelSecondarySubscriptions.Clear();

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
                .Select(item => new ListItemViewModel(item))
                .InvokeCommand(this.SelectItem)
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
                .Select(item => new ListItemViewModel(item))
                .InvokeCommand(this.SelectItem)
                .DisposeWith(this.sideViewModelSubscriptions);

            return viewModel;
        }

        private ReactiveObject CreateMovieForm(Movie movie)
        {
            this.Log().Debug($"Creating a form for movie: {movie}");

            var form = new MovieFormViewModel(movie, this.Kinds, this.FileName);

            form.Save
                .Select(m => new MovieListItem(m))
                .Do(this.source.AddOrUpdate)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(item => this.SelectedItem = this.Items.First(vm => vm.Item == item))
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Save
                .Discard()
                .InvokeCommand(this.Save)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Close
                .SubscribeAsync(async () => await this.SelectItem.Execute())
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .Subscribe(this.RemoveMovie)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .Discard()
                .InvokeCommand(form.Close);

            return form;
        }

        private ReactiveObject CreateSeriesForm(Series series)
        {
            this.Log().Debug($"Creating a form for series: {series}");

            var form = new SeriesFormViewModel(series, this.Kinds, this.FileName);

            form.Save
                .Select(s => new SeriesListItem(s))
                .Do(this.source.AddOrUpdate)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(item => this.SelectedItem = this.Items.First(vm => vm.Item == item))
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Save
                .Discard()
                .InvokeCommand(this.Save)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Close
                .SubscribeAsync(async () => await this.SelectItem.Execute())
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .Subscribe(this.RemoveSeries)
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
            SeriesComponentFormViewModelBase<TM, TVm> form,
            SeriesFormViewModel seriesForm)
            where TM : EntityBase
            where TVm : SeriesComponentFormViewModelBase<TM, TVm>
        {
            form.Close
                .SubscribeAsync(async () => await this.SelectItem.Execute())
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

        private void RemoveMovie(Movie movie)
        {
            this.source.RemoveKey(new MovieListItem(movie).Id);

            if (movie.Entry != null)
            {
                this.RemoveMovieSeriesEntry(movie.Entry);
            }
        }

        private void RemoveSeries(Series series)
        {
            this.source.RemoveKey(new SeriesListItem(series).Id);

            if (series.Entry != null)
            {
                this.RemoveMovieSeriesEntry(series.Entry);
            }
        }

        private void RemoveMovieSeriesEntry(MovieSeriesEntry movieSeriesEntry)
        {
            var movieSeries = movieSeriesEntry.ParentSeries;

            movieSeries.Entries
                .Where(entry => entry.SequenceNumber >= movieSeriesEntry.SequenceNumber)
                .Select(entry => entry.Movie != null
                    ? new MovieListItem(entry.Movie)
                    : entry.Series != null
                        ? (ListItem)new SeriesListItem(entry.Series)
                        : new MovieSeriesListItem(entry.MovieSeries!))
                .ForEach(this.source.AddOrUpdate);

            if (movieSeries.Entries.Count == 0 && movieSeries.ShowTitles)
            {
                this.source.RemoveKey(new MovieSeriesListItem(movieSeries).Id);

                if (movieSeries.Entry != null)
                {
                    this.RemoveMovieSeriesEntry(movieSeries.Entry);
                }
            }
        }
    }
}
