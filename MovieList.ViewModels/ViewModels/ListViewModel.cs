using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

using DynamicData;

using MovieList.Comparers;
using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.ListItems;

using ReactiveUI;

using Splat;

namespace MovieList.ViewModels
{
    public sealed class ListViewModel : ReactiveObject
    {
        private readonly ISourceCache<ListItem, string> source;
        private readonly ReadOnlyObservableCollection<ListItemViewModel> items;

        public ListViewModel(string fileName, IList<Kind> kinds, IListService? listService = null)
        {
            this.FileName = fileName;

            listService ??= Locator.Current.GetService<IListService>(fileName);

            this.source = new SourceCache<ListItem, string>(item => item.Id);

            this.source.PopulateFrom(
                Observable.FromAsync(() => listService.GetListAsync(kinds))
                    .Select(this.ToListItems));

            this.source.Connect()
                .Filter(item => !String.IsNullOrEmpty(item.Title))
                .Sort(ListItemComparer.Instance)
                .Transform(item => new ListItemViewModel(item))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.items)
                .DisposeMany()
                .Subscribe();
        }

        public string FileName { get; }

        public ReadOnlyObservableCollection<ListItemViewModel> Items
            => this.items;

        private IEnumerable<ListItem> ToListItems(
            (IEnumerable<Movie> Movies, IEnumerable<Series> Series, IEnumerable<MovieSeries> MovieSeries) list)
            => list.Movies.Select(movie => new MovieListItem(movie))
                .Cast<ListItem>()
                .Concat(list.Series.Select(series => new SeriesListItem(series)))
                .Concat(list.MovieSeries.Select(movieSeries => new MovieSeriesListItem(movieSeries)));
    }
}
