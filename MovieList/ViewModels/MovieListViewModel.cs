using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MovieList.Commands;
using MovieList.Config;
using MovieList.Data.Models;
using MovieList.Events;
using MovieList.Services;
using MovieList.ViewModels.ListItems;
using MovieList.Views;

namespace MovieList.ViewModels
{
    public class MovieListViewModel : ViewModelBase
    {
        private readonly App app;
        private readonly IDbService dbService;
        private readonly IOptions<Configuration> config;

        private ObservableCollection<ListItem> items;

        public MovieListViewModel(
            App app,
            IDbService dbService,
            IOptions<Configuration> config,
            SidePanelViewModel sidePanel)
        {
            this.app = app;
            this.dbService = dbService;
            this.config = config;
            this.SidePanel = sidePanel;

            sidePanel.Closed += (sender, e) =>
                this.MovieListControl.List.UnselectAll();

            this.SelectItem = new DelegateCommand<ListItem>(this.OnSelectItem);
            this.AddItem = new DelegateCommand<EntityBase>(this.OnAddItem);
            this.UpdateItem = new DelegateCommand<EntityBase>(this.OnUpdateItem);
            this.DeleteItem = new DelegateCommand<EntityBase>(this.OnDeleteItem);

            this.MarkAsWatched = new DelegateCommand<ListItem>(this.ToggleWatched, this.CanMarkAsWatched);
            this.MarkAsNotWatched = new DelegateCommand<ListItem>(this.ToggleWatched, this.CanMarkAsNotWatched);
            this.MarkAsReleased = new DelegateCommand<MovieListItem>(this.ToggleReleased, this.CanMarkAsReleased);
            this.MarkAsNotReleased = new DelegateCommand<MovieListItem>(this.ToggleReleased, this.CanMarkAsNotReleased);
        }

        public ICommand SelectItem { get; }
        public ICommand AddItem { get; }
        public ICommand UpdateItem { get; }
        public ICommand DeleteItem { get; }

        public ICommand MarkAsWatched { get; }
        public ICommand MarkAsNotWatched { get; }
        public ICommand MarkAsReleased { get; }
        public ICommand MarkAsNotReleased { get; }

        public MovieListControl MovieListControl { get; set; }

        public ObservableCollection<ListItem> Items
        {
            get => this.items;
            set
            {
                this.items = value;
                this.OnPropertyChanged();
            }
        }

        private SidePanelViewModel SidePanel { get; }

        public event EventHandler<ListItemUpdatedEventArgs> ListItemUpdated;

        public async Task LoadItemsAsync()
        {
            this.Items = new ObservableCollection<ListItem>(await this.dbService.LoadListAsync());
            app.Dispatcher.Invoke(() => this.MovieListControl.LoadingProgressBar.Visibility = Visibility.Collapsed);
        }

        public bool MoveSelectedItem(Key key)
        {
            bool result = false;

            if (this.MovieListControl.List.SelectedItem != null)
            {
                switch (key)
                {
                    case Key.Up:
                        this.SelectAndScroll(
                            (this.MovieListControl.List.SelectedIndex + this.Items.Count - 1) % this.Items.Count);
                        result = true;
                        break;
                    case Key.Down:
                        this.SelectAndScroll(
                            (this.MovieListControl.List.SelectedIndex + this.Items.Count + 1) % this.Items.Count);
                        result = true;
                        break;
                    case Key.Home:
                        this.SelectAndScroll(0);
                        result = true;
                        break;
                    case Key.End:
                        this.SelectAndScroll(this.Items.Count - 1);
                        result = true;
                        break;
                }
            } else if (key == Key.Up || key == Key.Home)
            {
                this.SelectAndScroll(this.Items.Count - 1);
                result = true;
            } else if (key == Key.Down || key == Key.End)
            {
                this.SelectAndScroll(0);
                result = true;
            }

            return result;
        }

        protected virtual void OnListItemUpdated(ListItem item)
            => this.ListItemUpdated?.Invoke(this, new ListItemUpdatedEventArgs(item));

        private void SelectAndScroll(int index)
        {
            this.MovieListControl.List.SelectedIndex = index;

            this.MovieListControl.List.UpdateLayout();
            this.MovieListControl.List.ScrollIntoView(this.MovieListControl.List.SelectedItem);
        }

        private void OnSelectItem(ListItem item)
        {
            item.OpenSidePanel(this.SidePanel);
            this.MovieListControl.List.SelectedIndex = Util.BinarySearchIndexOf(this.Items, item);
            this.MovieListControl.List.Focus();
        }

        private void OnAddItem(EntityBase entity)
        {
            switch (entity)
            {
                case Movie movie:
                    var movieItem = new MovieListItem(movie, this.config.Value);
                    int movieIndex = Util.BinarySearchIndexOf(this.items, movieItem);
                    if (movieIndex < 0)
                    {
                        movieIndex = ~movieIndex;
                    }

                    this.Items.Insert(movieIndex, movieItem);
                    this.MovieListControl.List.SelectedIndex = movieIndex;

                    this.MovieListControl.List.UpdateLayout();
                    this.MovieListControl.List.ScrollIntoView(this.MovieListControl.List.SelectedItem);
                    break;
                case Series series:
                    var seriesItem = new SeriesListItem(series, this.config.Value);
                    int seriesIndex = Util.BinarySearchIndexOf(this.items, seriesItem);
                    if (seriesIndex < 0)
                    {
                        seriesIndex = ~seriesIndex;
                    }

                    this.Items.Insert(seriesIndex, seriesItem);
                    this.MovieListControl.List.SelectedIndex = seriesIndex;

                    this.MovieListControl.List.UpdateLayout();
                    this.MovieListControl.List.ScrollIntoView(this.MovieListControl.List.SelectedItem);
                    break;
            }
        }

        private void OnUpdateItem(EntityBase entity)
        {
            this.OnDeleteItem(entity);
            this.OnAddItem(entity);
        }

        private void OnDeleteItem(EntityBase entity)
        {
            switch (entity)
            {
                case Movie movie:
                    this.Items.RemoveAt(Util.BinarySearchIndexOf(this.items, new MovieListItem(movie, this.config.Value)));
                    break;
                case Series series:
                    this.Items.RemoveAt(Util.BinarySearchIndexOf(this.items, new SeriesListItem(series, this.config.Value)));
                    break;
            }
        }

        private async void ToggleWatched(ListItem item)
        {
            await this.dbService.ToggleWatchedAsync(item);
            this.UpdateColor(item, app.ServiceProvider.GetService<IOptions<Configuration>>().Value);
            this.OnListItemUpdated(item);
        }

        private bool CanMarkAsWatched(ListItem item)
            => item switch
            {
                MovieListItem movieItem => !movieItem.Movie.IsWatched,
                SeriesListItem seriesItem => !seriesItem.Series.IsWatched,
                _ => false
            };

        private bool CanMarkAsNotWatched(ListItem item)
            => item switch
            {
                MovieListItem movieItem => movieItem.Movie.IsWatched,
                SeriesListItem seriesItem => seriesItem.Series.IsWatched,
                _ => false
            };

        private async void ToggleReleased(MovieListItem item)
        {
            await this.dbService.ToggleReleasedAsync(item);
            this.UpdateColor(item, app.ServiceProvider.GetService<IOptions<Configuration>>().Value);
            this.OnListItemUpdated(item);
        }

        private bool CanMarkAsReleased(MovieListItem item)
            => !item.Movie.IsReleased && item.Movie.Year <= DateTime.Now.Year;

        private bool CanMarkAsNotReleased(MovieListItem item)
            => item.Movie.IsReleased && item.Movie.Year >= DateTime.Now.Year;

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
