using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using DynamicData;

using MovieList.Comparers;
using MovieList.Data;
using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.ListItems;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace MovieList.ViewModels
{
    public sealed class ListViewModel : ReactiveObject
    {
        private readonly ISourceCache<ListItem, string> source;
        private readonly ReadOnlyObservableCollection<ListItemViewModel> items;
        private readonly Subject<Unit> resort = new Subject<Unit>();

        public ListViewModel(
            string fileName,
            ReadOnlyObservableCollection<Kind> kinds,
            Settings? settings = null,
            IListService? listService = null)
        {
            settings ??= Locator.Current.GetService<Settings>(fileName);
            listService ??= Locator.Current.GetService<IListService>(fileName);

            this.source = new SourceCache<ListItem, string>(item => item.Id);

            this.MovieList = listService.GetList(kinds);
            this.source.AddOrUpdate(this.MovieList.ToListItems());
            this.Log().Debug($"Loaded the list of {this.source.Count} items");

            this.source.Connect()
                .Filter(item => !String.IsNullOrEmpty(item.Title))
                .AutoRefresh(item => item.DisplayNumber)
                .AutoRefresh(item => item.Title)
                .AutoRefresh(item => item.OriginalTitle)
                .AutoRefresh(item => item.Year)
                .Transform(item => new ListItemViewModel(item))
                .Sort(
                    new PropertyComparer<ListItemViewModel, ListItem>(
                        vm => vm.Item, new ListItemTitleComparer(settings.CultureInfo)),
                    this.resort)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.items)
                .Subscribe();

            this.SelectItem = ReactiveCommand.Create<ListItem?, bool>(this.OnSelectItem);
            this.PreviewSelectItem = ReactiveCommand.Create<ListItemViewModel, ListItemViewModel>(vm => vm);
            this.ForceSelectedItem = ReactiveCommand.Create(() => { });
            this.AddOrUpdate = ReactiveCommand.Create<ListItem, ListItem>(this.OnAddOrUpdate);
            this.RemoveMovie = ReactiveCommand.Create<Movie>(this.OnRemoveMovie);
            this.RemoveSeries = ReactiveCommand.Create<Series>(this.OnRemoveSeries);
            this.RemoveMovieSeries = ReactiveCommand.Create<MovieSeries>(this.OnRemoveMovieSeries);

            this.AddOrUpdate.InvokeCommand(this.SelectItem);
        }

        public ReadOnlyObservableCollection<ListItemViewModel> Items
            => this.items;

        [Reactive]
        public Data.MovieList MovieList { get; private set; }

        [Reactive]
        public ListItemViewModel? SelectedItem { get; set; }

        public ReactiveCommand<ListItem?, bool> SelectItem { get; }
        public ReactiveCommand<ListItemViewModel, ListItemViewModel> PreviewSelectItem { get; }
        public ReactiveCommand<Unit, Unit> ForceSelectedItem { get; }

        public ReactiveCommand<ListItem, ListItem> AddOrUpdate { get; }

        public ReactiveCommand<Movie, Unit> RemoveMovie { get; }
        public ReactiveCommand<Series, Unit> RemoveSeries { get; }
        public ReactiveCommand<MovieSeries, Unit> RemoveMovieSeries { get; }

        public ListItem EntryToListItem(MovieSeriesEntry entry)
            => entry.Movie != null
                ? new MovieListItem(entry.Movie)
                : entry.Series != null
                    ? (ListItem)new SeriesListItem(entry.Series)
                    : new MovieSeriesListItem(entry.MovieSeries!);

        private bool OnSelectItem(ListItem? item)
        {
            bool isSame = this.SelectedItem?.Item.Id == item?.Id;

            this.SelectedItem = this.Items.FirstOrDefault(vm => vm.Item == item);

            return !isSame || item == null;
        }

        private ListItem OnAddOrUpdate(ListItem item)
        {
            this.source.Edit(list =>
            {
                list.AddOrUpdate(item);

                var movieSeries = this.GetMovieSeries(item);

                if (movieSeries != null)
                {
                    movieSeries.Entries
                        .Select(this.EntryToListItem)
                        .ForEach(list.AddOrUpdate);

                    this.UpdateParentSeries(movieSeries, list);
                }
            });

            this.Resort();

            return item;
        }

        private MovieSeries? GetMovieSeries(ListItem item)
            => item switch
            {
                MovieListItem movieItem when movieItem.Movie.Entry != null => movieItem.Movie.Entry.ParentSeries,
                SeriesListItem seriesItem when seriesItem.Series.Entry != null => seriesItem.Series.Entry.ParentSeries,
                MovieSeriesListItem movieSeriesItem => movieSeriesItem.MovieSeries,
                _ => null
            };

        private void OnRemoveMovie(Movie movie)
        {
            this.source.Edit(list =>
            {
                list.RemoveKey(new MovieListItem(movie).Id);

                if (movie.Entry != null)
                {
                    this.RemoveMovieSeriesEntry(movie.Entry, list);
                }
            });

            this.Resort();
        }

        private void OnRemoveSeries(Series series)
        {
            this.source.Edit(list =>
            {
                list.RemoveKey(new SeriesListItem(series).Id);

                if (series.Entry != null)
                {
                    this.RemoveMovieSeriesEntry(series.Entry, list);
                }
            });

            this.Resort();
        }

        private void OnRemoveMovieSeries(MovieSeries movieSeries)
        {
            this.source.Edit(list =>
            {
                list.RemoveKey(new MovieSeriesListItem(movieSeries).Id);

                if (movieSeries.Entry != null)
                {
                    this.RemoveMovieSeriesEntry(movieSeries.Entry, list);
                }

                foreach (var entry in movieSeries.Entries)
                {
                    list.AddOrUpdate(this.EntryToListItem(entry));
                }
            });

            this.Resort();
        }

        private void RemoveMovieSeriesEntry(MovieSeriesEntry movieSeriesEntry, ISourceUpdater<ListItem, string> list)
        {
            var movieSeries = movieSeriesEntry.ParentSeries;

            movieSeries.Entries
                .OrderBy(entry => entry.SequenceNumber)
                .SkipWhile(entry => entry.SequenceNumber < movieSeriesEntry.SequenceNumber)
                .Select(this.EntryToListItem)
                .ForEach(list.AddOrUpdate);

            this.UpdateParentSeries(movieSeries, list);

            if (movieSeries.Entries.Count == 0 && movieSeries.ShowTitles)
            {
                list.RemoveKey(new MovieSeriesListItem(movieSeries).Id);

                if (movieSeries.Entry != null)
                {
                    this.RemoveMovieSeriesEntry(movieSeries.Entry, list);
                }
            }
        }

        private void Resort()
            => this.resort.OnNext(Unit.Default);

        private void UpdateParentSeries(MovieSeries movieSeries, ISourceUpdater<ListItem, string> list)
        {
            movieSeries.Entry?.ParentSeries.Entries
                .OrderBy(entry => entry.SequenceNumber)
                .SkipWhile(entry => entry.SequenceNumber <= movieSeries.Entry.SequenceNumber)
                .TakeWhile(entry => entry.MovieSeries != null && entry.MovieSeries.MergeDisplayNumbers)
                .SelectMany(entry => entry.MovieSeries!.Entries)
                .Select(this.EntryToListItem)
                .ForEach(list.AddOrUpdate);
        }
    }
}
