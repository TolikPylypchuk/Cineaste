using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

using HandyControl.Controls;

using MovieList.Commands;
using MovieList.Properties;
using MovieList.ViewModels.FormItems;
using MovieList.Views;

using MessageBox = System.Windows.MessageBox;

namespace MovieList.ViewModels
{
    public class SeasonFormViewModel : ViewModelBase
    {
        private SeasonFormItem season;

        public SeasonFormViewModel(SeriesFormViewModel parentForm, SidePanelViewModel sidePanel)
        {
            this.ParentForm = parentForm;
            this.SidePanel = sidePanel;

            this.Save = new DelegateCommand(this.OnSave, () => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(() => this.Season.RevertChanges(), () => this.CanCancelChanges);
            this.Delete = new DelegateCommand(this.OnDelete);
        }

        public ICommand Save { get; }
        public ICommand Cancel { get; }
        public ICommand Delete { get; }

        public SeasonFormControl SeasonFormControl { get; set; }

        public SeasonFormItem Season
        {
            get => this.season;
            set
            {
                this.season = value;
                this.season.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.Season));

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(FormTitle));
            }
        }

        public string FormTitle
            => this.season.Season.Title.Name;

        public bool CanSaveChanges
            => this.Season.AreChangesPresent &&
                !this.Season.HasErrors &&
                !this.Season.Titles.Any(t => t.HasErrors) &&
                !this.Season.OriginalTitles.Any(t => t.HasErrors) &&
                !this.Season.Periods.Any(t => t.HasErrors);

        public bool CanCancelChanges
            => this.Season.AreChangesPresent;

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
            this.Season.ClearEmptyTitles();

            if (!this.SeasonFormControl
                .FindVisualChildren<TextBox>()
                .Select(textBox => textBox.VerifyData())
                .Aggregate(true, (a, b) => a && b))
            {
                return;
            }

            this.Season.WriteChanges();
            this.ParentForm.AreComponentsChanged = true;
        }

        private void OnDelete()
        {
            var result = MessageBox.Show(
                Messages.DeleteSeasonPrompt,
                Messages.Delete,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.ParentForm.Series.Components.Remove(this.Season);
                this.ParentForm.SeasonsToDelete.Add(this.Season.Season);

                foreach (var season in this.Season.Season.Series.Seasons
                    .Where(s => s.OrdinalNumber > this.Season.OrdinalNumber))
                {
                    season.OrdinalNumber--;
                }

                foreach (var episode in this.Season.Season.Series.SpecialEpisodes
                    .Where(e => e.OrdinalNumber > this.Season.OrdinalNumber))
                {
                    episode.OrdinalNumber--;
                }

                this.SidePanel.GoUpToSeries.ExecuteIfCan(null);
            }
        }
    }
}
