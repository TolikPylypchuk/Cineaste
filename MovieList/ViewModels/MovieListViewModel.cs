using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MovieList.Config;
using MovieList.Data.Models;
using MovieList.Events;
using MovieList.Services;
using MovieList.ViewModels.ListItems;

namespace MovieList.ViewModels
{
    public class MovieListViewModel : ViewModelBase
    {
        private readonly App app;
        private readonly IOptions<Configuration> config;

        private ObservableCollection<ListItem> items = new ObservableCollection<ListItem>();

        public MovieListViewModel(App app, IOptions<Configuration> config, SidePanelViewModel sidePanel)
        {
            this.app = app;
            this.config = config;
            this.SidePanel = sidePanel;

            this.SelectItem = new DelegateCommand(this.OnSelectItem);
            this.AddItem = new DelegateCommand(this.OnAddItem);
            this.UpdateItem = new DelegateCommand(this.OnUpdateItem);
            this.DeleteItem = new DelegateCommand(this.OnDeleteItem);

            this.MarkAsWatched = new DelegateCommand(this.ToggleWatched, this.CanMarkAsWatched);
            this.MarkAsNotWatched = new DelegateCommand(this.ToggleWatched, this.CanMarkAsNotWatched);
            this.MarkAsReleased = new DelegateCommand(this.ToggleReleased, this.CanMarkAsReleased);
            this.MarkAsNotReleased = new DelegateCommand(this.ToggleReleased, this.CanMarkAsNotReleased);
        }

        public ICommand SelectItem { get; }
        public ICommand AddItem { get; }
        public ICommand UpdateItem { get; }
        public ICommand DeleteItem { get; }

        public ICommand MarkAsWatched { get; }
        public ICommand MarkAsNotWatched { get; }
        public ICommand MarkAsReleased { get; }
        public ICommand MarkAsNotReleased { get; }

        private SidePanelViewModel SidePanel { get; }

        public ObservableCollection<ListItem> Items
        {
            get => this.items;
            set
            {
                this.items = value;
                this.OnPropertyChanged();
            }
        }

        public event EventHandler<ListItemUpdatedEventArgs> ListItemUpdated;

        public async Task LoadItemsAsync()
        {
            using var scope = app.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMovieService>();
            this.Items = await service.LoadListAsync();
        }

        protected virtual void OnListItemUpdated(ListItem item)
            => this.ListItemUpdated?.Invoke(this, new ListItemUpdatedEventArgs(item));

        private void OnSelectItem(object obj)
        {
            if (obj is ListItem item)
            {
                item.OpenSidePanel(this.SidePanel);
            }
        }

        private void OnAddItem(object obj)
        {
            switch (obj)
            {
                case Movie movie:
                    var item = new MovieListItem(movie, this.config.Value);
                    int index = Util.BinarySearchIndexOf(this.items, item);
                    if (index < 0)
                    {
                        index = ~index;
                    }

                    this.Items.Insert(index, item);
                    break;
            }
        }

        private void OnUpdateItem(object obj)
        {
            this.OnDeleteItem(obj);
            this.OnAddItem(obj);
        }

        private void OnDeleteItem(object obj)
        {
            switch (obj)
            {
                case Movie movie:
                    this.Items.RemoveAt(Util.BinarySearchIndexOf(this.items, new MovieListItem(movie, config.Value)));
                    break;
            }
        }

        private async void ToggleWatched(object obj)
        {
            if (obj is ListItem item)
            {
                using var scope = app.ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IMovieService>();
                await service.ToggleWatchedAsync(item);
                this.UpdateColor(item, app.ServiceProvider.GetService<IOptions<Configuration>>().Value);
                this.OnListItemUpdated(item);
            }
        }

        private bool CanMarkAsWatched(object obj)
            => obj switch
            {
                MovieListItem movieItem => !movieItem.Movie.IsWatched,
                SeriesListItem seriesItem => !seriesItem.Series.IsWatched,
                _ => false
            };

        private bool CanMarkAsNotWatched(object obj)
            => obj switch
            {
                MovieListItem movieItem => movieItem.Movie.IsWatched,
                SeriesListItem seriesItem => seriesItem.Series.IsWatched,
                _ => false
            };

        private async void ToggleReleased(object obj)
        {
            if (obj is MovieListItem item)
            {
                using var scope = app.ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IMovieService>();
                await service.ToggleReleasedAsync(item);
                this.UpdateColor(item, app.ServiceProvider.GetService<IOptions<Configuration>>().Value);
                this.OnListItemUpdated(item);
            }
        }

        private bool CanMarkAsReleased(object obj)
            => obj is MovieListItem item ? !item.Movie.IsReleased && item.Movie.Year <= DateTime.Now.Year : false;

        private bool CanMarkAsNotReleased(object obj)
            => obj is MovieListItem item ? item.Movie.IsReleased && item.Movie.Year >= DateTime.Now.Year : false;

        private void UpdateColor(ListItem item, Configuration? config)
        {
            item.UpdateColor(config);

            var parentItem = item switch
            {
                MovieListItem movieItem when movieItem.Movie.Entry != null =>
                    this.Items.FirstOrDefault(i =>
                        i is MovieSeriesListItem movieSeriesItem &&
                        movieSeriesItem.MovieSeries.Id == movieItem.Movie.Entry.MovieSeriesId),
                SeriesListItem seriesItem when seriesItem.Series.Entry != null =>
                    this.Items.FirstOrDefault(i =>
                            i is MovieSeriesListItem movieSeriesItem &&
                            movieSeriesItem.MovieSeries.Id == seriesItem.Series.Entry.MovieSeriesId),
                MovieSeriesListItem movieSeriesItem when movieSeriesItem.MovieSeries.ParentSeries != null =>
                    this.Items.FirstOrDefault(i =>
                        i is MovieSeriesListItem movieSeriesItem &&
                        movieSeriesItem.MovieSeries.Id == movieSeriesItem.MovieSeries.ParentSeriesId),
                _ => null
            };

            if (parentItem != null)
            {
                this.UpdateColor(parentItem, config);
            }
        }
    }
}
