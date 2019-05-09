using System.Windows;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;

using MovieList.Data.Models;
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
            this.Close = new DelegateCommand(_ => this.OnClose());
        }

        public ICommand OpenMovie { get; }
        public ICommand OpenSeries { get; }
        public ICommand OpenMovieSeries { get; }
        public ICommand Close { get; }

        public SidePanelControl SidePanelControl { get; set; }

        private void OnOpenMovie(object obj)
        {
            if (obj is Movie movie)
            {
                var control = new MovieFormControl();
                control.DataContext = control.ViewModel = this.app.ServiceProvider.GetRequiredService<MovieFormViewModel>();
                control.ViewModel.Movie = new MovieFormItem(movie);
                this.SidePanelControl.ContentContainer.Content = control;
            }
        }

        private void OnOpenSeries(object obj)
        {
        }

        public void OnOpenMovieSeries(object obj)
        {
        }

        public void OnClose()
        {
            var control = new AddNewControl();
            control.DataContext = control.ViewModel = this.app.ServiceProvider.GetRequiredService<AddNewViewModel>();
            this.SidePanelControl.ContentContainer.Content = control;
        }
    }
}
