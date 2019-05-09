using System.Threading.Tasks;
using System.Windows.Input;

using MovieList.Data.Models;
using MovieList.Properties;

namespace MovieList.ViewModels
{
    public class MovieFormViewModel : ViewModelBase
    {
        private Movie movie;

        public MovieFormViewModel()
        {
            this.Save = new DelegateCommand(async _ => await this.SaveAsync(), _ => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(async _ => await this.CancelAsync(), _ => this.CanCancelChanges);
        }

        public ICommand Save { get; }
        public ICommand Cancel { get; }

        public Movie Movie
        {
            get => this.movie;
            set
            {
                this.movie = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(FormTitle));
            }
        }

        public string FormTitle
            => this.movie.Id != default ? this.movie.Title.Name : Messages.NewMovie;

        public bool CanSaveChanges => true;
        public bool CanCancelChanges => true;

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
    }
}
