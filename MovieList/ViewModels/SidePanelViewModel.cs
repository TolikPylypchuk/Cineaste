using System;
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
        }

        public ICommand OpenMovie { get; }
        public ICommand OpenSeries { get; }
        public ICommand OpenMovieSeries { get; }
        public ICommand OpenSeriesComponent { get; }
        public ICommand Close { get; }
        public ICommand GoUpToSeries { get; }

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

        public void OnOpenMovieSeries(MovieSeries movieSeries)
        {
        }

        public void OnOpenSeriesComponent(SeriesComponentFormItemBase component)
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

        public void OnClose()
        {
            var control = new AddNewControl();
            control.DataContext = control.ViewModel = this.serviceProvider.GetRequiredService<AddNewViewModel>();
            this.SidePanelControl.ContentContainer.Content = control;

            this.Closed?.Invoke(this, EventArgs.Empty);
        }

        public void OnGoUpToSeries()
        {
            this.SidePanelControl.ContentContainer.Content = this.parentFormControl;
            this.parentFormControl = null;
        }

        public bool CanGoUpToSeries()
            => this.parentFormControl != null;
    }
}
