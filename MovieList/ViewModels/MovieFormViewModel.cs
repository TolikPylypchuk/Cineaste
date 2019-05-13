using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

using MovieList.Events;
using MovieList.Properties;
using MovieList.ViewModels.FormItems;
using MovieList.ViewModels.ListItems;

namespace MovieList.ViewModels
{
    public class MovieFormViewModel : ViewModelBase
    {
        private MovieFormItem movie;
        private ObservableCollection<KindViewModel> allKinds;

        public MovieFormViewModel(MovieListViewModel movieList, SidePanelViewModel sidePanel, SettingsViewModel settings)
        {
            this.SidePanel = sidePanel;

            this.Save = new DelegateCommand(async _ => await this.SaveAsync(), _ => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(async _ => await this.CancelAsync(), _ => this.CanCancelChanges);

            movieList.ListItemUpdated += this.OnListItemUpdated;
            settings.SettingsUpdated += this.OnSettingsUpdated;
        }

        public ICommand Save { get; }
        public ICommand Cancel { get; }

        public MovieFormItem Movie
        {
            get => this.movie;
            set
            {
                this.movie = value;
                this.movie.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(Movie));
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(FormTitle));
            }
        }

        public ObservableCollection<KindViewModel> AllKinds
        {
            get => this.allKinds;
            set
            {
                this.allKinds = value;
                this.OnPropertyChanged();
            }
        }

        public SidePanelViewModel SidePanel { get; }

        public string FormTitle
            => this.movie.Movie.Id != default ? this.movie.Movie.Title.Name : Messages.NewMovie;

        public bool CanSaveChanges
            => this.Movie.AreChangesPresent &&
                !this.Movie.HasErrors &&
                !this.Movie.Titles.Any(t => t.HasErrors) &&
                !this.Movie.OriginalTitles.Any(t => t.HasErrors);

        public bool CanCancelChanges
            => this.Movie.AreChangesPresent;

        public bool CanSaveOrCancelChanges
            => this.CanSaveChanges || this.CanCancelChanges;

        public Task SaveAsync()
        {
            return Task.CompletedTask;
        }

        public Task CancelAsync()
        {
            return Task.CompletedTask;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName != nameof(this.AllKinds))
            {
                base.OnPropertyChanged(nameof(CanSaveChanges));
                base.OnPropertyChanged(nameof(CanCancelChanges));
                base.OnPropertyChanged(nameof(CanSaveOrCancelChanges));
            }
        }

        private void OnListItemUpdated(object sender, ListItemUpdatedEventArgs e)
        {
            if (e.Item is MovieListItem item)
            {
                this.Movie.IsWatched = item.Movie.IsWatched;
                this.Movie.IsReleased = item.Movie.IsReleased;
            }
        }

        private void OnSettingsUpdated(object sender, SettingsUpdatedEventArgs e)
        {
            this.AllKinds = new ObservableCollection<KindViewModel>(e.Kinds);
            this.Movie.Kind = this.AllKinds.First(k => k.Kind.Id == this.Movie.Kind.Kind.Id);
        }
    }
}
