using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

using MovieList.Commands;
using MovieList.Controls;
using MovieList.Properties;
using MovieList.ViewModels.FormItems;
using MovieList.Views;

using MessageBox = System.Windows.MessageBox;

namespace MovieList.ViewModels
{
    public class SpecialEpisodeFormViewModel : ViewModelBase
    {
        private SpecialEpisodeFormItem episode;

        public SpecialEpisodeFormViewModel(SeriesFormViewModel parentForm, SidePanelViewModel sidePanel)
        {
            this.ParentForm = parentForm;
            this.SidePanel = sidePanel;

            this.Save = new DelegateCommand(this.OnSave, () => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(() => this.SpecialEpisode.RevertChanges(), () => this.CanCancelChanges);
            this.Delete = new DelegateCommand(this.OnDelete, this.CanDelete);
        }

        public DelegateCommand Save { get; }
        public DelegateCommand Cancel { get; }
        public DelegateCommand Delete { get; }

        public SpecialEpisodeFormControl SpecialEpisodeFormControl { get; set; }

        public SpecialEpisodeFormItem SpecialEpisode
        {
            get => this.episode;
            set
            {
                this.episode = value;
                this.episode.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.SpecialEpisode));

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(FormTitle));
            }
        }

        public string FormTitle
            => !String.IsNullOrEmpty(this.episode.SpecialEpisode.Title.Name)
                ? this.episode.SpecialEpisode.Title.Name
                : Messages.NewSpecialEpisode;

        public bool CanSaveChanges
            => this.SpecialEpisode.AreChangesPresent &&
                !this.SpecialEpisode.HasErrors &&
                !this.SpecialEpisode.Titles.Any(t => t.HasErrors) &&
                !this.SpecialEpisode.OriginalTitles.Any(t => t.HasErrors);

        public bool CanCancelChanges
            => this.SpecialEpisode.AreChangesPresent;

        public bool CanSaveOrCancelChanges
            => this.CanSaveChanges || this.CanCancelChanges;

        public SeriesFormViewModel ParentForm { get; }
        public SidePanelViewModel SidePanel { get; }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            base.OnPropertyChanged(nameof(this.CanSaveChanges));
            base.OnPropertyChanged(nameof(this.CanCancelChanges));
            base.OnPropertyChanged(nameof(this.CanSaveOrCancelChanges));
        }

        private void OnSave()
        {
            this.SpecialEpisode.ClearEmptyTitles();

            if (!this.SpecialEpisodeFormControl
                .FindVisualChildren<TextBox>()
                .Select(textBox => textBox.VerifyData())
                .Aggregate(true, (a, b) => a && b))
            {
                return;
            }

            this.SpecialEpisode.WriteChanges();
            this.ParentForm.AreComponentsChanged = true;
        }

        private void OnDelete()
        {
            var result = MessageBox.Show(
                Messages.DeleteSpecialEpisodePrompt,
                Messages.Delete,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.ParentForm.Series.Components.Remove(this.SpecialEpisode);
                this.ParentForm.EpisodesToDelete.Add(this.SpecialEpisode.SpecialEpisode);

                foreach (var season in this.SpecialEpisode.SpecialEpisode.Series.Seasons
                    .Where(s => s.OrdinalNumber > this.SpecialEpisode.OrdinalNumber))
                {
                    season.OrdinalNumber--;
                }

                foreach (var episode in this.SpecialEpisode.SpecialEpisode.Series.SpecialEpisodes
                    .Where(e => e.OrdinalNumber > this.SpecialEpisode.OrdinalNumber))
                {
                    episode.OrdinalNumber--;
                }

                this.SidePanel.GoUpToSeries.ExecuteIfCan();
            }
        }

        private bool CanDelete()
            => this.SpecialEpisode.SpecialEpisode.Id != default;
    }
}
