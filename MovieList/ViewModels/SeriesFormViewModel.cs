using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using HandyControl.Controls;

using Microsoft.Extensions.Options;

using MovieList.Config;
using MovieList.Data.Models;
using MovieList.Events;
using MovieList.Properties;
using MovieList.Services;
using MovieList.ViewModels.FormItems;
using MovieList.Views;

using MessageBox = System.Windows.MessageBox;

namespace MovieList.ViewModels
{
    public class SeriesFormViewModel : ViewModelBase
    {
        private readonly IDbService dbService;
        private readonly IOptions<Configuration> config;

        private SeriesFormItem series;
        private ObservableCollection<KindViewModel> allKinds;
        private bool areComponentsChanged;

        public SeriesFormViewModel(
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

            this.Save = new DelegateCommand(async _ => await this.SaveAsync(), _ => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(_ => this.OnCancel(), _ => this.CanCancelChanges);
            this.Delete = new DelegateCommand(async _ => await this.DeleteAsync(), _ => this.CanDelete());
            this.AddSeason = new DelegateCommand(_ => this.OnAddSeason());
            this.AddSpecialEpisode = new DelegateCommand(_ => this.OnAddSpecialEpisode());

            settings.SettingsUpdated += this.OnSettingsUpdated;
        }

        public ICommand Save { get; }
        public ICommand Cancel { get; }
        public ICommand Delete { get; }
        public ICommand AddSeason { get; }
        public ICommand AddSpecialEpisode { get; }

        public SeriesFormControl SeriesFormControl { get; set; }

        public SeriesFormItem Series
        {
            get => this.series;
            set
            {
                this.series = value;
                this.series.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.Series));

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(FormTitle));
            }
        }

        public string FormTitle
            => this.series.Series.Id != default ? this.series.Series.Title.Name : Messages.NewSeries;

        public ObservableCollection<KindViewModel> AllKinds
        {
            get => this.allKinds;
            set
            {
                this.allKinds = value;
                this.OnPropertyChanged();
            }
        }

        public bool AreComponentsChanged
        {
            get => this.areComponentsChanged;
            set
            {
                this.areComponentsChanged = value;
                this.OnPropertyChanged();
            }
        }

        public MovieListViewModel MovieList { get; }
        public SidePanelViewModel SidePanel { get; }

        public bool CanSaveChanges
            => (this.Series.AreChangesPresent || this.AreComponentsChanged) &&
                !this.Series.HasErrors &&
                !this.Series.Titles.Any(t => t.HasErrors) &&
                !this.Series.OriginalTitles.Any(t => t.HasErrors) &&
                this.Series.Components.Any() &&
                !this.Series.Components.Any(c => c.HasErrors) &&
                !this.Series.Components.Any(c => c.AreChangesPresent);

        public bool CanCancelChanges
            => this.Series.AreChangesPresent || this.AreComponentsChanged;

        public bool CanSaveOrCancelChanges
            => this.CanSaveChanges || this.CanCancelChanges;

        public List<Season> SeasonsToDelete { get; } = new List<Season>();
        public List<SpecialEpisode> EpisodesToDelete { get; } = new List<SpecialEpisode>();

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
            this.Series.ClearEmptyTitles();

            if (!this.SeriesFormControl
                .FindVisualChildren<TextBox>()
                .Select(textBox => textBox.VerifyData())
                .Aggregate(true, (a, b) => a && b))
            {
                return;
            }

            var titlesToDelete = this.Series.RemovedTitles
                .Union(this.Series.Components.SelectMany(c => c.RemovedTitles))
                .Select(t => t.Title)
                .ToList();

            this.Series.WriteChanges();

            bool shouldAddToList = this.Series.Series.Id == default;

            await this.dbService.SaveSeriesAsync(
                this.Series.Series, this.SeasonsToDelete, this.EpisodesToDelete, titlesToDelete);

            this.SeasonsToDelete.Clear();
            this.EpisodesToDelete.Clear();

            (shouldAddToList ? this.MovieList.AddItem : this.MovieList.UpdateItem).ExecuteIfCan(this.Series.Series);

            this.AreComponentsChanged = false;
        }

        private void OnCancel()
        {
            this.Series.RevertChanges();
            this.AreComponentsChanged = false;
        }

        private async Task DeleteAsync()
        {
            var result = MessageBox.Show(
                Messages.DeleteSeriesPrompt,
                Messages.Delete,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.MovieList.DeleteItem.ExecuteIfCan(this.Series.Series);
                await this.dbService.DeleteAsync(this.Series.Series);
                this.SidePanel.Close.ExecuteIfCan(null);
            }
        }

        private bool CanDelete()
            => this.Series.Series.Id != default;

        private void OnAddSeason()
        {
            var newSeason = new Season
            {
                Series = this.Series.Series,
                OrdinalNumber = this.Series.Components.Count + 1,
                IsReleased = true,
                Channel = this.Series.Components
                    .OrderByDescending(c => c.OrdinalNumber)
                    .FirstOrDefault()
                    ?.Channel
                    ?? String.Empty
            };

            newSeason.Periods = new List<Period>
            {
                new Period
                {
                    Season = newSeason,
                    StartMonth = 1,
                    StartYear = 2000,
                    EndMonth = 1,
                    EndYear = 2000
                }
            };

            var seasonNumber = this.Series.Components.OfType<SeasonFormItem>().Count() + 1;

            newSeason.Titles = new List<Title>
            {
                new Title
                {
                    Season = newSeason,
                    Name = this.config.Value.DefaultSeasonTitle
                        .Replace(Messages.DefaultSeasonNumberPlaceholder, seasonNumber.ToString()),
                    IsOriginal = false,
                    Priority = 1
                },
                new Title
                {
                    Season = newSeason,
                    Name = this.config.Value.DefaultSeasonOriginalTitle
                        .Replace(Messages.DefaultSeasonNumberPlaceholder, seasonNumber.ToString()),
                    IsOriginal = true,
                    Priority = 1
                }
            };

            var formItem = new SeasonFormItem(newSeason);
            this.Series.Components.Add(formItem);
            this.SidePanel.OpenSeriesComponent.ExecuteIfCan(formItem);
        }

        private void OnAddSpecialEpisode()
        {
            var newEpisode = new SpecialEpisode
            {
                Series = this.Series.Series,
                Month = 1,
                Year = 2000,
                IsReleased = true,
                OrdinalNumber = this.Series.Components.Count + 1,
                Channel = this.Series.Components
                    .OrderByDescending(c => c.OrdinalNumber)
                    .FirstOrDefault()
                    ?.Channel
                    ?? String.Empty
            };

            newEpisode.Titles = new List<Title>
            {
                new Title
                {
                    SpecialEpisode = newEpisode,
                    Name = String.Empty,
                    IsOriginal = false,
                    Priority = 1
                },
                new Title
                {
                    SpecialEpisode = newEpisode,
                    Name = String.Empty,
                    IsOriginal = true,
                    Priority = 1
                }
            };

            var formItem = new SpecialEpisodeFormItem(newEpisode);
            this.Series.Components.Add(formItem);
            this.SidePanel.OpenSeriesComponent.ExecuteIfCan(formItem);
        }
        
        private void OnSettingsUpdated(object sender, SettingsUpdatedEventArgs e)
        {
            this.AllKinds = new ObservableCollection<KindViewModel>(e.Kinds);
            this.Series.Kind = this.AllKinds.First(k => k.Kind.Id == this.Series.Kind.Kind.Id);
        }
    }
}
