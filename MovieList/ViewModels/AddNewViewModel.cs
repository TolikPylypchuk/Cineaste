using System;
using System.Collections.Generic;
using System.Windows.Input;

using MovieList.Data.Models;

namespace MovieList.ViewModels
{
    public class AddNewViewModel : ViewModelBase
    {
        private readonly SidePanelViewModel sidePanel;

        public AddNewViewModel(SidePanelViewModel sidePanelViewModel)
        {
            this.sidePanel = sidePanelViewModel;
            this.AddNewMovie = new DelegateCommand(_ => this.OnAddNewMovie());
            this.AddNewSeries = new DelegateCommand(_ => this.OnAddNewSeries());
        }

        public ICommand AddNewMovie { get; }
        public ICommand AddNewSeries { get; }

        public void OnAddNewMovie()
            => this.sidePanel.OpenMovie.ExecuteIfCan(new Movie
            {
                Titles = new List<Title>
                {
                    new Title { Name = String.Empty, IsOriginal = false, Priority = 1 },
                    new Title { Name = String.Empty, IsOriginal = true, Priority = 1 }
                },
                Year = 2000
            });

        public void OnAddNewSeries()
            => this.sidePanel.OpenSeries.ExecuteIfCan(new Series
            {
                Titles = new List<Title>
                {
                    new Title { Name = String.Empty, IsOriginal = false, Priority = 1 },
                    new Title { Name = String.Empty, IsOriginal = true, Priority = 1 }
                },
                Status = SeriesStatus.NotStarted
            });
    }
}
