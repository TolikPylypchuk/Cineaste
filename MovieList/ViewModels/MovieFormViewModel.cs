using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

using MovieList.Properties;
using MovieList.ViewModels.FormItems;

namespace MovieList.ViewModels
{
    public class MovieFormViewModel : ViewModelBase
    {
        private MovieFormItem movie;

        public MovieFormViewModel(SidePanelViewModel sidePanel)
        {
            this.SidePanel = sidePanel;

            this.Save = new DelegateCommand(async _ => await this.SaveAsync(), _ => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(async _ => await this.CancelAsync(), _ => this.CanCancelChanges);
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

        public SidePanelViewModel SidePanel { get; set; }

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
            base.OnPropertyChanged(nameof(CanSaveChanges));
            base.OnPropertyChanged(nameof(CanCancelChanges));
            base.OnPropertyChanged(nameof(CanSaveOrCancelChanges));
        }
    }
}
