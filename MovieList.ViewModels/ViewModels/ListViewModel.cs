using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

using DynamicData;

using MovieList.Comparers;
using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.ListItems;
using MovieList.ViewModels.Forms;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace MovieList.ViewModels
{
    public sealed class ListViewModel : ReactiveObject, IEnableLogger
    {
        private readonly ISourceCache<ListItem, string> source;
        private readonly ReadOnlyObservableCollection<ListItemViewModel> items;
        private CompositeDisposable sideViewModelSubscriptions;

        public ListViewModel(
            string fileName,
            ReadOnlyObservableCollection<Kind> kinds,
            IListService? listService = null)
        {
            this.FileName = fileName;
            this.Kinds = kinds;

            listService ??= Locator.Current.GetService<IListService>(fileName);

            this.sideViewModelSubscriptions = new CompositeDisposable();

            this.source = new SourceCache<ListItem, string>(item => item.Id);

            this.source.PopulateFrom(
                Observable.FromAsync(() => Task.Run(() => listService.GetListAsync(kinds)))
                    .Select(this.ToListItems));

            this.source.Connect()
                .Filter(item => !String.IsNullOrEmpty(item.Title))
                .Sort(ListItemComparer.Instance)
                .Transform(item => new ListItemViewModel(item))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.items)
                .DisposeMany()
                .Subscribe();

            this.WhenAnyValue(vm => vm.SelectedItem)
                .Subscribe(this.SelectItem);
        }

        public string FileName { get; }

        private ReadOnlyObservableCollection<Kind> Kinds { get; }

        public ReadOnlyObservableCollection<ListItemViewModel> Items
            => this.items;

        [Reactive]
        public ListItemViewModel? SelectedItem { get; set; }

        [Reactive]
        public ReactiveObject? SideViewModel { get; private set; }

        private IEnumerable<ListItem> ToListItems(
            (IEnumerable<Movie> Movies, IEnumerable<Series> Series, IEnumerable<MovieSeries> MovieSeries) list)
            => list.Movies.Select(movie => new MovieListItem(movie))
                .Cast<ListItem>()
                .Concat(list.Series.Select(series => new SeriesListItem(series)))
                .Concat(list.MovieSeries.Select(movieSeries => new MovieSeriesListItem(movieSeries)));

        private void SelectItem(ListItemViewModel? vm)
        {
            this.sideViewModelSubscriptions.Dispose();
            this.sideViewModelSubscriptions = new CompositeDisposable();

            this.SideViewModel = vm?.Item switch
            {
                MovieListItem movieItem => this.CreateMovieForm(movieItem.Movie),
                _ => null
            };
        }

        private MovieFormViewModel CreateMovieForm(Movie movie)
        {
            this.Log().Debug("Creating a form for movie: {movie}");

            var form = new MovieFormViewModel(movie, this.Kinds);

            form.Save
                .Select(m => new MovieListItem(m))
                .Subscribe(this.source.AddOrUpdate)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Close
                .Where(closed => closed)
                .Discard()
                .Subscribe(() => this.SelectedItem = null)
                .DisposeWith(this.sideViewModelSubscriptions);

            form.Delete
                .WhereNotNull()
                .Select(m => new MovieListItem(m))
                .Subscribe(item => this.source.RemoveKey(item.Id))
                .DisposeWith(this.sideViewModelSubscriptions);

            return form;
        }
    }
}
