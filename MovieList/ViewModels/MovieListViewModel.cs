using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MovieList.Config;
using MovieList.Events;
using MovieList.Services;
using MovieList.ViewModels.ListItems;

namespace MovieList.ViewModels
{
    public class MovieListViewModel : ViewModelBase
    {
        private readonly App app;
        private readonly SidePanelViewModel sidePanelViewModel;
        private ObservableCollection<ListItem> items = new ObservableCollection<ListItem>();

        public MovieListViewModel(App app, SidePanelViewModel sidePanelViewModel)
        {
            this.app = app;
            this.sidePanelViewModel = sidePanelViewModel;

            this.SelectItem = new DelegateCommand(this.OnSelectItem);
            this.MarkAsWatched = new DelegateCommand(this.ToggleWatched, this.CanMarkAsWatched);
            this.MarkAsNotWatched = new DelegateCommand(this.ToggleWatched, this.CanMarkAsNotWatched);
            this.MarkAsReleased = new DelegateCommand(this.ToggleReleased, this.CanMarkAsReleased);
            this.MarkAsNotReleased = new DelegateCommand(this.ToggleReleased, this.CanMarkAsNotReleased);
        }

        public ICommand SelectItem { get; }
        public ICommand MarkAsWatched { get; }
        public ICommand MarkAsNotWatched { get; }
        public ICommand MarkAsReleased { get; }
        public ICommand MarkAsNotReleased { get; }

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
            var service = scope.ServiceProvider.GetRequiredService<IMovieListService>();
            this.Items = await service.LoadAllItemsAsync();
        }

        protected virtual void OnListItemUpdated(ListItem item)
            => this.ListItemUpdated?.Invoke(this, new ListItemUpdatedEventArgs(item));

        private void OnSelectItem(object obj)
        {
            if (obj is ListItem item)
            {
                item.OpenSidePanel(this.sidePanelViewModel);
            }
        }

        private async void ToggleWatched(object obj)
        {
            if (obj is ListItem item)
            {
                using var scope = app.ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IMovieListService>();
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
                var service = scope.ServiceProvider.GetRequiredService<IMovieListService>();
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
