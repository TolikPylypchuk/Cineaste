using System;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;

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

            this.OpenMovie = new DelegateCommand(this.OnOpenMovie);
            this.OpenSeries = new DelegateCommand(this.OnOpenSeries);
            this.OpenMovieSeries = new DelegateCommand(this.OnOpenMovieSeries);
            this.OpenSeason = new DelegateCommand(this.OnOpenSeason);
            this.OpenSpecialEpisode = new DelegateCommand(this.OnOpenSpecialEpisode);
            this.Close = new DelegateCommand(_ => this.OnClose());
            this.GoUpToSeries = new DelegateCommand(_ => this.OnGoUpToSeries(), _ => this.CanGoUpToSeries());
        }

        public ICommand OpenMovie { get; }
        public ICommand OpenSeries { get; }
        public ICommand OpenMovieSeries { get; }
        public ICommand OpenSeason { get; }
        public ICommand OpenSpecialEpisode { get; }
        public ICommand Close { get; }
        public ICommand GoUpToSeries { get; }

        public SidePanelControl SidePanelControl { get; set; }

        public event EventHandler Closed;

        private async void OnOpenMovie(object obj)
        {
            if (obj is Movie movie)
            {
                var control = new MovieFormControl();
                control.DataContext = control.ViewModel =
                    this.serviceProvider.GetRequiredService<MovieFormViewModel>();
                control.ViewModel.MovieFormControl = control;

                control.ViewModel.AllKinds = await this.dbService.LoadAllKindsAsync();
                control.ViewModel.Movie = new MovieFormItem(movie, control.ViewModel.AllKinds);
                this.SidePanelControl.ContentContainer.Content = control;
            }
        }

        private async void OnOpenSeries(object obj)
        {
            if (obj is Series series)
            {
                var control = new SeriesFormControl();
                control.DataContext = control.ViewModel =
                    this.serviceProvider.GetRequiredService<SeriesFormViewModel>();
                control.ViewModel.SeriesFormControl = control;

                control.ViewModel.AllKinds = await this.dbService.LoadAllKindsAsync();
                control.ViewModel.Series = new SeriesFormItem(series, control.ViewModel.AllKinds);
                this.SidePanelControl.ContentContainer.Content = control;
            }
        }

        public void OnOpenMovieSeries(object obj)
        {
        }

        public void OnOpenSeason(object obj)
        {
            if (obj is SeasonFormItem season)
            {
                this.parentFormControl = this.SidePanelControl.ContentContainer.Content as SeriesFormControl;

                var control = new SeasonFormControl();
                control.DataContext = control.ViewModel =
                    this.serviceProvider.GetRequiredService<SeasonFormViewModel>();
                control.ViewModel.SeasonFormControl = control;

                control.ViewModel.Season = season;
                this.SidePanelControl.ContentContainer.Content = control;
            }
        }

        public void OnOpenSpecialEpisode(object obj)
        {
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
