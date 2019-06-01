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
        private readonly App app;

        public SidePanelViewModel(App app)
        {
            this.app = app;

            this.OpenMovie = new DelegateCommand(this.OnOpenMovie);
            this.OpenSeries = new DelegateCommand(this.OnOpenSeries);
            this.OpenMovieSeries = new DelegateCommand(this.OnOpenMovieSeries);
            this.OpenSeason = new DelegateCommand(this.OnOpenSeason);
            this.OpenSpecialEpisode = new DelegateCommand(this.OnOpenSpecialEpisode);
            this.Close = new DelegateCommand(_ => this.OnClose());
        }

        public ICommand OpenMovie { get; }
        public ICommand OpenSeries { get; }
        public ICommand OpenMovieSeries { get; }
        public ICommand OpenSeason { get; }
        public ICommand OpenSpecialEpisode { get; }
        public ICommand Close { get; }

        public SidePanelControl SidePanelControl { get; set; }

        public event EventHandler Closed;

        private async void OnOpenMovie(object obj)
        {
            if (obj is Movie movie)
            {
                var control = new MovieFormControl();
                control.DataContext = control.ViewModel = this.app.ServiceProvider.GetRequiredService<MovieFormViewModel>();
                control.ViewModel.MovieFormControl = control;

                using var scope = this.app.ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IKindService>();

                control.ViewModel.AllKinds = await service.LoadAllKindsAsync();
                control.ViewModel.Movie = new MovieFormItem(movie, control.ViewModel.AllKinds);
                this.SidePanelControl.ContentContainer.Content = control;
            }
        }

        private async void OnOpenSeries(object obj)
        {
            if (obj is Series series)
            {
                var control = new SeriesFormControl();
                control.DataContext = control.ViewModel = this.app.ServiceProvider.GetRequiredService<SeriesFormViewModel>();
                control.ViewModel.SeriesFormControl = control;

                using var scope = this.app.ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IKindService>();

                control.ViewModel.AllKinds = await service.LoadAllKindsAsync();
                control.ViewModel.Series = new SeriesFormItem(series, control.ViewModel.AllKinds);
                this.SidePanelControl.ContentContainer.Content = control;
            }
        }

        public void OnOpenMovieSeries(object obj)
        {
        }

        public void OnOpenSeason(object obj)
        {
        }

        public void OnOpenSpecialEpisode(object obj)
        {
        }

        public void OnClose()
        {
            var control = new AddNewControl();
            control.DataContext = control.ViewModel = this.app.ServiceProvider.GetRequiredService<AddNewViewModel>();
            this.SidePanelControl.ContentContainer.Content = control;

            this.Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
