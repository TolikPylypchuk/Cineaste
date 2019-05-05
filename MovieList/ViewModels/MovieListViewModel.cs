using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MovieList.Config;
using MovieList.Services;
using MovieList.ViewModels.ListItems;

namespace MovieList.ViewModels
{
    public class MovieListViewModel : ViewModelBase
    {
        private readonly App app;
        private ObservableCollection<ListItem> items = new ObservableCollection<ListItem>();

        public MovieListViewModel(App app)
        {
            this.app = app;
            this.SelectItem = new RelayCommand(this.OnSelectItem);
            this.MarkAsWatched = new RelayCommand(this.ToggleWatched, this.CanMarkAsWatched);
            this.MarkAsNotWatched = new RelayCommand(this.ToggleWatched, this.CanMarkAsNotWatched);
            this.MarkAsReleased = new RelayCommand(this.ToggleReleased, this.CanMarkAsReleased);
            this.MarkAsNotReleased = new RelayCommand(this.ToggleReleased, this.CanMarkAsNotReleased);
        }

        public RelayCommand SelectItem { get; }
        public RelayCommand MarkAsWatched { get; }
        public RelayCommand MarkAsNotWatched { get; }
        public RelayCommand MarkAsReleased { get; }
        public RelayCommand MarkAsNotReleased { get; }

        public ObservableCollection<ListItem> Items
        {
            get => this.items;
            set
            {
                this.items = value;
                this.OnPropertyChanged();
            }
        }

        public async Task LoadItemsAsync()
        {
            using var scope = app.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMovieListService>();
            this.Items = await service.LoadAllItemsAsync();
        }

        private void OnSelectItem(object obj)
        {
            if (obj is ListItem item)
            {
                MessageBox.Show(item.Title);
            }
        }

        private async void ToggleWatched(object obj)
        {
            if (obj is ListItem item)
            {
                using var scope = app.ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IMovieListService>();
                await service.ToggleWatchedAsync(item);
                this.UpdateColor(item);
            }
        }

        private bool CanMarkAsWatched(object obj)
            => obj switch
            {
                MovieListItem movieItem => !movieItem.Movie.IsWatched,
                SeriesListItem seriesItem => !seriesItem.Series.IsWatched,
                _ => false
            };

        public bool CanMarkAsNotWatched(object obj)
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
                this.UpdateColor(item);
            }
        }

        private bool CanMarkAsReleased(object obj)
            => obj is MovieListItem item ? !item.Movie.IsReleased && item.Movie.Year <= DateTime.Now.Year : false;

        public bool CanMarkAsNotReleased(object obj)
            => obj is MovieListItem item ? item.Movie.IsReleased && item.Movie.Year >= DateTime.Now.Year : false;

        public void UpdateColor(ListItem item)
        {
            item.UpdateColor(app.ServiceProvider.GetService<IOptions<Configuration>>().Value);

            switch (item)
            {
                case MovieListItem movieItem when movieItem.Movie.Entry != null:
                    this.UpdateColor(this.Items.First(i =>
                        i is MovieSeriesListItem movieSeriesItem &&
                        movieSeriesItem.MovieSeries.Id == movieItem.Movie.Entry!.MovieSeriesId));
                    break;
                case SeriesListItem seriesItem when seriesItem.Series.Entry != null:
                    this.UpdateColor(this.Items.First(i =>
                        i is MovieSeriesListItem movieSeriesItem &&
                        movieSeriesItem.MovieSeries.Id == seriesItem.Series.Entry!.MovieSeriesId));
                    break;
                case MovieSeriesListItem movieSeriesItem when movieSeriesItem.MovieSeries.ParentSeries != null:
                    this.UpdateColor(this.Items.First(i =>
                        i is MovieSeriesListItem movieSeriesItem &&
                        movieSeriesItem.MovieSeries.Id == movieSeriesItem.MovieSeries.ParentSeriesId));
                    break;
            }
        }
    }
}
