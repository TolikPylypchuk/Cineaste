using System;
using System.Linq;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;

using MovieList.Commands;
using MovieList.Data.Models;
using MovieList.Services;
using MovieList.ViewModels.FormItems;
using MovieList.Views;

namespace MovieList.ViewModels
{
    public class SidePanelViewModel : ViewModelBase
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IDbService dbService;

        private SeriesFormControl? parentFormControl;

        public SidePanelViewModel(IServiceProvider serviceProvider, IDbService dbService)
        {
            this.serviceProvider = serviceProvider;
            this.dbService = dbService;

            this.OpenMovie = new DelegateCommand<Movie>(this.OnOpenMovie);
            this.OpenSeries = new DelegateCommand<Series>(this.OnOpenSeries);
            this.OpenMovieSeries = new DelegateCommand<MovieSeries>(this.OnOpenMovieSeries);
            this.OpenSeriesComponent = new DelegateCommand<SeriesComponentFormItemBase>(this.OnOpenSeriesComponent);

            this.Close = new DelegateCommand(this.OnClose);

            this.GoUpToSeries = new DelegateCommand(this.OnGoUpToSeries, this.CanGoUpToSeries);
            this.GoUpToMovieSeries = new DelegateCommand<EntityBase?>(
                this.OnGoUpToMovieSeries, this.CanGoUpToMovieSeries);

            this.SelectNextEntry = new DelegateCommand<MovieSeriesEntry?>(
                this.OnSelectNextEntry, this.CanSelectNextEntry);
            this.SelectPreviousEntry = new DelegateCommand<MovieSeriesEntry?>(
                this.OnSelectPreviousEntry, this.CanSelectPreviousEntry);
        }

        public ICommand OpenMovie { get; }
        public ICommand OpenSeries { get; }
        public ICommand OpenMovieSeries { get; }
        public ICommand OpenSeriesComponent { get; }
        public ICommand Close { get; }
        public ICommand GoUpToSeries { get; }
        public ICommand GoUpToMovieSeries { get; }
        public ICommand SelectNextEntry { get; }
        public ICommand SelectPreviousEntry { get; }

        public SidePanelControl SidePanelControl { get; set; }

        public event EventHandler Closed;

        private async void OnOpenMovie(Movie movie)
        {
            var control = new MovieFormControl();
            control.DataContext = control.ViewModel =
                this.serviceProvider.GetRequiredService<MovieFormViewModel>();
            control.ViewModel.MovieFormControl = control;
            control.ViewModel.AllKinds = await this.dbService.LoadAllKindsAsync();
            control.ViewModel.Movie = new MovieFormItem(movie, control.ViewModel.AllKinds);

            this.SidePanelControl.ContentContainer.Content = control;
        }

        private async void OnOpenSeries(Series series)
        {
            var control = new SeriesFormControl();
            control.DataContext = control.ViewModel =
                this.serviceProvider.GetRequiredService<SeriesFormViewModel>();
            control.ViewModel.SeriesFormControl = control;
            control.ViewModel.AllKinds = await this.dbService.LoadAllKindsAsync();
            control.ViewModel.Series = new SeriesFormItem(series, control.ViewModel.AllKinds);

            this.SidePanelControl.ContentContainer.Content = control;
        }

        private void OnOpenMovieSeries(MovieSeries movieSeries)
        {
        }

        private void OnOpenSeriesComponent(SeriesComponentFormItemBase component)
        {
            switch (component)
            {
                case SeasonFormItem season:
                    if (this.SidePanelControl.ContentContainer.Content is SeriesFormControl seriesFormControl1)
                    {
                        this.parentFormControl = seriesFormControl1;
                    }

                    var seasonFormControl = new SeasonFormControl();
                    seasonFormControl.DataContext = seasonFormControl.ViewModel =
                        this.serviceProvider.GetRequiredService<SeasonFormViewModel>();
                    seasonFormControl.ViewModel.SeasonFormControl = seasonFormControl;
                    seasonFormControl.ViewModel.Season = season;

                    this.SidePanelControl.ContentContainer.Content = seasonFormControl;
                    break;
                case SpecialEpisodeFormItem episode:
                    if (this.SidePanelControl.ContentContainer.Content is SeriesFormControl seriesFormControl2)
                    {
                        this.parentFormControl = seriesFormControl2;
                    }

                    var specialEpisodeFormControl = new SpecialEpisodeFormControl();
                    specialEpisodeFormControl.DataContext = specialEpisodeFormControl.ViewModel =
                        this.serviceProvider.GetRequiredService<SpecialEpisodeFormViewModel>();
                    specialEpisodeFormControl.ViewModel.SpecialEpisodeFormControl = specialEpisodeFormControl;
                    specialEpisodeFormControl.ViewModel.SpecialEpisode = episode;

                    this.SidePanelControl.ContentContainer.Content = specialEpisodeFormControl;
                    break;
            }
        }

        private void OnClose()
        {
            var control = new AddNewControl();
            control.DataContext = control.ViewModel = this.serviceProvider.GetRequiredService<AddNewViewModel>();
            this.SidePanelControl.ContentContainer.Content = control;

            this.Closed?.Invoke(this, EventArgs.Empty);
        }

        private void OnGoUpToSeries()
        {
            this.SidePanelControl.ContentContainer.Content = this.parentFormControl;
            this.parentFormControl = null;
        }

        private bool CanGoUpToSeries()
            => this.parentFormControl != null;

        private void OnGoUpToMovieSeries(EntityBase? entity)
        {
            switch (entity)
            {
                case MovieSeriesEntry entry:
                    this.OnOpenMovieSeries(entry.MovieSeries);
                    break;
                case MovieSeries movieSeries:
                    this.OnOpenMovieSeries(movieSeries.ParentSeries!);
                    break;
            }
        }

        private bool CanGoUpToMovieSeries(EntityBase? entity)
            => entity switch
            {
                MovieSeriesEntry _ => true,
                MovieSeries movieSeries => movieSeries.ParentSeries != null,
                _ => false
            };

        private void OnSelectNextEntry(MovieSeriesEntry? entry)
        {
            if (entry == null)
            {
                return;
            }

            var movieSeries = entry.MovieSeries;

            var nextEntry = movieSeries.Entries
                .Where(e => e.OrdinalNumber > entry.OrdinalNumber)
                .OrderBy(e => e.OrdinalNumber)
                .FirstOrDefault();

            if (nextEntry != null && nextEntry.OrdinalNumber == entry.OrdinalNumber + 1)
            {
                this.OpenEntry(nextEntry);
            } else
            {
                var nextPart = movieSeries.Parts
                    .Where(p => p.OrdinalNumber > entry.OrdinalNumber)
                    .OrderBy(p => p.OrdinalNumber)
                    .FirstOrDefault();

                if (nextPart != null)
                {
                    this.OpenEntry(nextPart.GetFirstEntry());
                }
            }
        }

        private bool CanSelectNextEntry(MovieSeriesEntry? entry)
            => entry != null && (entry.MovieSeries.Entries.Any(e => e.OrdinalNumber > entry.OrdinalNumber) ||
                entry.MovieSeries.Parts.Any(p => p.OrdinalNumber! > entry.OrdinalNumber));

        private void OnSelectPreviousEntry(MovieSeriesEntry? entry)
        {
            if (entry == null)
            {
                return;
            }

            var movieSeries = entry.MovieSeries;

            var previousEntry = movieSeries.Entries
                .Where(e => e.OrdinalNumber < entry.OrdinalNumber)
                .OrderByDescending(e => e.OrdinalNumber)
                .FirstOrDefault();

            if (previousEntry != null && previousEntry.OrdinalNumber == entry.OrdinalNumber - 1)
            {
                this.OpenEntry(previousEntry);
            } else
            {
                var previousPart = movieSeries.Parts
                    .Where(p => p.OrdinalNumber < entry.OrdinalNumber)
                    .OrderByDescending(p => p.OrdinalNumber)
                    .FirstOrDefault();

                if (previousPart != null)
                {
                    this.OpenEntry(previousPart.GetFirstEntry());
                }
            }
        }

        private bool CanSelectPreviousEntry(MovieSeriesEntry? entry)
            => entry != null && (entry.MovieSeries.Entries.Any(e => e.OrdinalNumber < entry.OrdinalNumber) ||
                entry.MovieSeries.Parts.Any(p => p.OrdinalNumber! < entry.OrdinalNumber));

        private void OpenEntry(MovieSeriesEntry entry)
        {
            if (entry.Movie != null)
            {
                this.OnOpenMovie(entry.Movie);
            } else
            {
                this.OnOpenSeries(entry.Series!);
            }
        }
    }
}
