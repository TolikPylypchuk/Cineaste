using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using HandyControl.Controls;

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

        private SeriesFormItem series;
        private ObservableCollection<KindViewModel> allKinds;

        public SeriesFormViewModel(
            IDbService dbService,
            MovieListViewModel movieList,
            SidePanelViewModel sidePanel,
            SettingsViewModel settings)
        {
            this.dbService = dbService;

            this.MovieList = movieList;
            this.SidePanel = sidePanel;

            this.Save = new DelegateCommand(async _ => await this.SaveAsync(), _ => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(_ => this.Series.RevertChanges(), _ => this.CanCancelChanges);
            this.Delete = new DelegateCommand(async _ => await this.DeleteAsync(), _ => this.CanDelete());

            settings.SettingsUpdated += this.OnSettingsUpdated;
        }

        public ICommand Save { get; }
        public ICommand Cancel { get; }
        public ICommand Delete { get; }

        public SeriesFormControl SeriesFormControl { get; set; }

        public SeriesFormItem Series
        {
            get => this.series;
            set
            {
                this.series = value;
                this.OnPropertyChanged();

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

        public MovieListViewModel MovieList { get; }
        public SidePanelViewModel SidePanel { get; }

        public bool CanSaveChanges
            => this.Series.AreChangesPresent &&
                !this.Series.HasErrors &&
                !this.Series.Titles.Any(t => t.HasErrors) &&
                !this.Series.OriginalTitles.Any(t => t.HasErrors);

        public bool CanCancelChanges
            => this.Series.AreChangesPresent;

        public bool CanSaveOrCancelChanges
            => this.CanSaveChanges || this.CanCancelChanges;

        public async Task SaveAsync()
        {
            this.Series.ClearEmptyTitles();

            if (!this.SeriesFormControl
                .FindVisualChildren<TextBox>()
                .Select(textBox => textBox.VerifyData())
                .Aggregate(true, (a, b) => a && b))
            {
                return;
            }

            this.Series.WriteChanges();

            bool shouldAddToList = this.Series.Series.Id == default;

            await this.dbService.SaveSeriesAsync(this.Series.Series);

            (shouldAddToList ? this.MovieList.AddItem : this.MovieList.UpdateItem).ExecuteIfCan(this.Series.Series);
        }

        public async Task DeleteAsync()
        {
            var result = MessageBox.Show(Messages.DeleteSeriesPrompt, Messages.Delete, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await this.dbService.DeleteAsync(this.Series.Series);

                this.SidePanel.Close.ExecuteIfCan(null);
                this.MovieList.DeleteItem.ExecuteIfCan(this.Series.Series);
            }
        }

        public bool CanDelete()
            => this.Series.Series.Id != default;

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

        private void OnSettingsUpdated(object sender, SettingsUpdatedEventArgs e)
        {
            this.AllKinds = new ObservableCollection<KindViewModel>(e.Kinds);
            this.Series.Kind = this.AllKinds.First(k => k.Kind.Id == this.Series.Kind.Kind.Id);
        }
    }
}
