using System;
using System.Collections.Generic;
using System.Windows.Input;

using MovieList.Data.Models;

namespace MovieList.ViewModels
{
    public class AddNewViewModel : ViewModelBase
    {
        private readonly SidePanelViewModel sidePanelViewModel;

        public AddNewViewModel(SidePanelViewModel sidePanelViewModel)
        {
            this.sidePanelViewModel = sidePanelViewModel;
            this.AddNewMovie = new DelegateCommand(_ => this.OnAddNewMovie());
            this.AddNewSeries = new DelegateCommand(_ => this.OnAddNewSeries());
        }

        public ICommand AddNewMovie { get; }
        public ICommand AddNewSeries { get; }

        public void OnAddNewMovie()
        {
            var newMovie = new Movie
            {
                Titles = new List<Title>
                {
                    new Title { Name = String.Empty, IsOriginal = false, Priority = 1 },
                    new Title { Name = String.Empty, IsOriginal = true, Priority = 1 }
                }
            };

            if (this.sidePanelViewModel.OpenMovie.CanExecute(newMovie))
            {
                this.sidePanelViewModel.OpenMovie.Execute(newMovie);
            }
        }

        public void OnAddNewSeries()
        {
            var newSeries = new Series();

            if (this.sidePanelViewModel.OpenSeries.CanExecute(newSeries))
            {
                this.sidePanelViewModel.OpenSeries.Execute(newSeries);
            }
        }
    }
}
