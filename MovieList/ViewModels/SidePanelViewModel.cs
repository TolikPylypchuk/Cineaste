using System.Windows;
using System.Windows.Input;

using MovieList.Data.Models;
using MovieList.Views;

namespace MovieList.ViewModels
{
    public class SidePanelViewModel : ViewModelBase
    {
        public SidePanelViewModel()
        {
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
                MessageBox.Show(movie.Titles.Count != 0 ? movie.Title.Name : "Movie");
            }
        }

        private void OnOpenSeries(object obj)
        {
            if (obj is Series series)
            {
                MessageBox.Show(series.Titles.Count != 0 ? series.Title.Name : "Series");
            }
        }

        public void OnOpenMovieSeries(object obj)
        {
            if (obj is MovieSeries movieSeries)
            {
                MessageBox.Show(movieSeries.Titles.Count != 0 ? movieSeries.Title?.Name : "Movie Series");
            }
        }

        public void OnClose()
        {
        }
    }
}
