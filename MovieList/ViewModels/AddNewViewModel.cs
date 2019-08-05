using System;
using System.Collections.Generic;

using MovieList.Commands;
using MovieList.Data.Models;

namespace MovieList.ViewModels
{
    public class AddNewViewModel : ViewModelBase
    {
        public AddNewViewModel(SidePanelViewModel sidePanelViewModel)
        {
            this.SidePanel = sidePanelViewModel;

            this.AddNewMovie = new DelegateCommand(this.OnAddNewMovie);
            this.AddNewSeries = new DelegateCommand(this.OnAddNewSeries);
        }

        public DelegateCommand AddNewMovie { get; }
        public DelegateCommand AddNewSeries { get; }

        private SidePanelViewModel SidePanel { get; }

        public void OnAddNewMovie()
            => this.SidePanel.OpenMovie.ExecuteIfCan(new Movie
            {
                Titles = new List<Title>
                {
                    new Title { Name = String.Empty, IsOriginal = false, Priority = 1 },
                    new Title { Name = String.Empty, IsOriginal = true, Priority = 1 }
                },
                Year = 2000
            });

        public void OnAddNewSeries()
            => this.SidePanel.OpenSeries.ExecuteIfCan(new Series
            {
                Titles = new List<Title>
                {
                    new Title { Name = String.Empty, IsOriginal = false, Priority = 1 },
                    new Title { Name = String.Empty, IsOriginal = true, Priority = 1 }
                },
                Status = SeriesStatus.Running
            });
    }
}
