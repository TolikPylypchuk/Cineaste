using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MovieList.Commands;
using MovieList.Config;
using MovieList.Controls;
using MovieList.Data.Models;
using MovieList.Events;
using MovieList.Services;
using MovieList.ViewModels.FormItems;
using MovieList.ViewModels.ListItems;
using MovieList.Views;

namespace MovieList.ViewModels
{
    public class MovieSeriesFormViewModel : ViewModelBase
    {
        private readonly IDbService dbService;
        private readonly IOptions<Configuration> config;

        private MovieSeriesFormItem movieSeries;
        private ObservableCollection<KindViewModel> allKinds;

        public MovieSeriesFormViewModel(
            IDbService dbService,
            IOptions<Configuration> config,
            MovieListViewModel movieList,
            SidePanelViewModel sidePanel,
            SettingsViewModel settings)
        {
            this.dbService = dbService;
            this.config = config;
            this.MovieList = movieList;
            this.SidePanel = sidePanel;

            this.Save = new DelegateCommand(async () => await this.SaveAsync(), () => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(this.OnCancel, () => this.CanCancelChanges);

            this.OpenForm = new DelegateCommand<MovieSeriesComponentFormItemBase>(this.OnOpenForm);
            this.ShowOrdinalNumber = new DelegateCommand<MovieSeriesComponentFormItemBase>(
                this.OnShowOrdinalNumber, this.CanShowOrdinalNumber);
            this.HideOrdinalNumber = new DelegateCommand<MovieSeriesComponentFormItemBase>(
                this.OnHideOrdinalNumber, this.CanHideOrdinalNumber);
            this.MoveComponentUp = new DelegateCommand<MovieSeriesComponentFormItemBase>(
                this.OnMoveComponentUp, this.CanMoveComponentUp);
            this.MoveComponentDown = new DelegateCommand<MovieSeriesComponentFormItemBase>(
                this.OnMoveComponentDown, this.CanMoveComponentDown);
            this.DetachComponent = new DelegateCommand<MovieSeriesComponentFormItemBase>(this.OnDetachComponent);

            this.AddMovie = new DelegateCommand(this.OnAddMovie);
            this.AddSeries = new DelegateCommand(this.OnAddSeries);

            this.AttachMovie = new DelegateCommand<Movie>(this.OnAttachMovie);
            this.AttachSeries = new DelegateCommand<Series>(this.OnAttachSeries);
            this.AttachMovieSeries = new DelegateCommand<MovieSeries>(this.OnAttachMovieSeries);

            settings.SettingsUpdated += this.OnSettingsUpdated;
        }

        public DelegateCommand Save { get; }
        public DelegateCommand Cancel { get; }

        public DelegateCommand<MovieSeriesComponentFormItemBase> OpenForm { get; }
        public DelegateCommand<MovieSeriesComponentFormItemBase> ShowOrdinalNumber { get; }
        public DelegateCommand<MovieSeriesComponentFormItemBase> HideOrdinalNumber { get; }
        public DelegateCommand<MovieSeriesComponentFormItemBase> MoveComponentUp { get; }
        public DelegateCommand<MovieSeriesComponentFormItemBase> MoveComponentDown { get; }
        public DelegateCommand<MovieSeriesComponentFormItemBase> DetachComponent { get; }

        public DelegateCommand AddMovie { get; }
        public DelegateCommand AddSeries { get; }

        public DelegateCommand<Movie> AttachMovie { get; }
        public DelegateCommand<Series> AttachSeries { get; }
        public DelegateCommand<MovieSeries> AttachMovieSeries { get; }

        public MovieSeriesFormItem MovieSeries
        {
            get => this.movieSeries;
            set
            {
                this.movieSeries = value;
                this.movieSeries.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.MovieSeries));

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.FormTitle));
            }
        }

        public MovieSeriesFormControl MovieSeriesFormControl { get; set; }

        public MovieListViewModel MovieList { get; }
        public SidePanelViewModel SidePanel { get; }

        public string FormTitle
            => this.MovieSeries.MovieSeries.GetTitleName();

        public ObservableCollection<KindViewModel> AllKinds
        {
            get => this.allKinds;
            set
            {
                this.allKinds = value;
                this.OnPropertyChanged();
            }
        }

        public bool CanSaveChanges
            => this.MovieSeries.AreChangesPresent &&
                !this.MovieSeries.HasErrors &&
                !this.MovieSeries.Titles.Any(t => t.HasErrors) &&
                !this.MovieSeries.OriginalTitles.Any(t => t.HasErrors);

        public bool CanCancelChanges
            => this.MovieSeries.AreChangesPresent;

        public bool CanSaveOrCancelChanges
            => this.CanSaveChanges || this.CanCancelChanges;

        public bool CanAddComponent
            => this.MovieSeries.MovieSeries.Id != default;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName != nameof(this.AllKinds))
            {
                base.OnPropertyChanged(nameof(this.CanSaveChanges));
                base.OnPropertyChanged(nameof(this.CanCancelChanges));
                base.OnPropertyChanged(nameof(this.CanSaveOrCancelChanges));
            }
        }

        private async Task SaveAsync()
        {
            this.MovieSeries.ClearEmptyTitles();

            if (!this.MovieSeriesFormControl
                .FindVisualChildren<TextBox>()
                .Select(textBox => textBox.VerifyData())
                .Aggregate(true, (a, b) => a && b))
            {
                return;
            }

            var titlesToDelete = this.MovieSeries.RemovedTitles.Select(t => t.Title).ToList();

            var entriesToDelete = this.MovieSeries.DetachedComponents
                .OfType<MovieSeriesEntryFormItemBase>()
                .Select(item => item.MovieSeriesEntry!);

            var partsToDetach = this.MovieSeries.DetachedComponents
                    .OfType<MovieSeriesFormItem>()
                    .Select(item => item.MovieSeries);

            var hadName = this.MovieSeries.MovieSeries.Title != null;
            var hasName = this.MovieSeries.HasName;

            this.MovieSeries.WriteChanges();

            await this.dbService.SaveMovieSeriesAsync(
                this.MovieSeries.MovieSeries, titlesToDelete, entriesToDelete, partsToDetach);

            var action = (hadName, hasName) switch
            {
                (true, true) => this.MovieList.UpdateItem,
                (false, true) => this.MovieList.AddItem,
                (true, false) => this.MovieList.DeleteItem,
                _ => DelegateCommand<EntityBase>.DoNothing
            };

            action.ExecuteIfCan(this.MovieSeries.MovieSeries);
            this.UpdateItems(this.MovieSeries.MovieSeries);

            if (this.MovieSeries.HasName)
            {
                this.MovieList.SelectItem.ExecuteIfCan(
                    new MovieSeriesListItem(this.MovieSeries.MovieSeries, this.config.Value));
            } else
            {
                this.SidePanel.OpenMovieSeries.ExecuteIfCan(this.MovieSeries.MovieSeries);
            }
        }

        private void OnCancel()
        {
            if (this.MovieSeries.MovieSeries.Id != default)
            {
                this.MovieSeries.RevertChanges();
            } else
            {
                var entry = this.MovieSeries.MovieSeries.GetFirstEntry();

                if (entry.Movie != null)
                {
                    entry.Movie.Entry = null;
                    this.SidePanel.OpenMovie.ExecuteIfCan(entry.Movie);
                } else
                {
                    entry.Series!.Entry = null;
                    this.SidePanel.OpenSeries.ExecuteIfCan(entry.Series!);
                }
            }
        }

        private void OnOpenForm(MovieSeriesComponentFormItemBase component)
        {
            if (this.MovieSeries.MovieSeries.Id == default)
            {
                var entry = this.MovieSeries.MovieSeries.GetFirstEntry();

                if (entry.Movie != null)
                {
                    entry.Movie.Entry = null;
                } else
                {
                    entry.Series!.Entry = null;
                }
            }

            this.MovieList.SelectItem.ExecuteIfCan(component.ToListItem(this.config.Value));
        }

        private void OnShowOrdinalNumber(MovieSeriesComponentFormItemBase component)
        {
            component.DisplayNumber = component.OrdinalNumber;

            foreach (var c in this.MovieSeries.Components
                .Where(c => c.OrdinalNumber > component.OrdinalNumber && c.DisplayNumber != null))
            {
                c.DisplayNumber++;
            }
        }

        private bool CanShowOrdinalNumber(MovieSeriesComponentFormItemBase component)
            => component.DisplayNumber == null;

        private void OnHideOrdinalNumber(MovieSeriesComponentFormItemBase component)
        {
            component.DisplayNumber = null;

            foreach (var c in this.MovieSeries.Components
                .Where(c => c.OrdinalNumber > component.OrdinalNumber && c.DisplayNumber != null))
            {
                c.DisplayNumber--;
            }
        }

        private bool CanHideOrdinalNumber(MovieSeriesComponentFormItemBase component)
            => !this.MovieSeries.IsLooselyConnected && component.DisplayNumber != null;

        private void OnMoveComponentUp(MovieSeriesComponentFormItemBase component)
        {
        }

        private bool CanMoveComponentUp(MovieSeriesComponentFormItemBase component)
            => component.OrdinalNumber != 1;

        private void OnMoveComponentDown(MovieSeriesComponentFormItemBase component)
        {
        }

        private bool CanMoveComponentDown(MovieSeriesComponentFormItemBase component)
            => component.OrdinalNumber != this.MovieSeries.Components.Count;

        private void OnDetachComponent(MovieSeriesComponentFormItemBase component)
        {
        }

        private void OnAddMovie()
        {
            var movie = new Movie
            {
                Titles = new List<Title>
                {
                    new Title { Name = String.Empty, IsOriginal = false, Priority = 1 },
                    new Title { Name = String.Empty, IsOriginal = true, Priority = 1 }
                },
                Year = 2000,
                Entry = new MovieSeriesEntry
                {
                    MovieSeries = this.MovieSeries.MovieSeries,
                    OrdinalNumber = this.MovieSeries.Components
                        .Max(c => c.OrdinalNumber) + 1,
                    DisplayNumber = this.MovieSeries.Components
                        .Where(c => c.DisplayNumber != null)
                        .Max(c => c.DisplayNumber) + 1
                }
            };

            movie.Entry.Movie = movie;

            this.SidePanel.OpenMovie.ExecuteIfCan(movie);
        }

        private void OnAddSeries()
        {
            var series = new Series
            {
                Titles = new List<Title>
                {
                    new Title { Name = String.Empty, IsOriginal = false, Priority = 1 },
                    new Title { Name = String.Empty, IsOriginal = true, Priority = 1 }
                },
                Status = SeriesStatus.Running,
                Entry = new MovieSeriesEntry
                {
                    MovieSeries = this.MovieSeries.MovieSeries,
                    OrdinalNumber = this.MovieSeries.Components
                        .OrderByDescending(c => c.OrdinalNumber)
                        .First()
                        .OrdinalNumber + 1,
                    DisplayNumber = this.MovieSeries.Components
                        .Where(c => c.DisplayNumber != null)
                        .OrderByDescending(c => c.DisplayNumber)
                        .First()
                        .DisplayNumber + 1
                }
            };

            series.Entry.Series = series;

            this.SidePanel.OpenSeries.ExecuteIfCan(series);
        }

        private void OnAttachMovie(Movie movie)
        {
        }

        private void OnAttachSeries(Series series)
        {
        }

        private void OnAttachMovieSeries(MovieSeries movieSeries)
        {
        }

        private void UpdateItems(MovieSeries movieSeries)
        {
            foreach (var entry in movieSeries.Entries)
            {
                this.MovieList.UpdateItem.ExecuteIfCan(entry.Movie != null ? (EntityBase)entry.Movie : entry.Series!);
            }

            foreach (var part in movieSeries.Parts)
            {
                if (part.Title != null)
                {
                    this.MovieList.UpdateItem.ExecuteIfCan(part);
                }

                this.UpdateItems(part);
            }
        }

        private void OnSettingsUpdated(object? sender, SettingsUpdatedEventArgs? e)
        {
            this.AllKinds = new ObservableCollection<KindViewModel>(e?.Kinds);
            this.MovieSeries.AllKinds = this.AllKinds;
        }
    }
}
