using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

using MovieList.Commands;
using MovieList.Controls;
using MovieList.Data.Models;
using MovieList.Services;
using MovieList.ViewModels.FormItems;
using MovieList.Views;

namespace MovieList.ViewModels
{
    public class MovieSeriesFormViewModel : ViewModelBase
    {
        private readonly IDbService dbService;

        private MovieSeriesFormItem movieSeries;

        public MovieSeriesFormViewModel(
            IDbService dbService,
            MovieListViewModel movieList,
            SidePanelViewModel sidePanel)
        {
            this.dbService = dbService;
            this.MovieList = movieList;
            this.SidePanel = sidePanel;

            this.Save = new DelegateCommand(async () => await this.SaveAsync(), () => this.CanSaveChanges);
            this.Cancel = new DelegateCommand(() => this.MovieSeries.RevertChanges(), () => this.CanCancelChanges);
        }

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

        public ICommand Save { get; }
        public ICommand Cancel { get; }

        public MovieSeriesFormControl MovieSeriesFormControl { get; set; }

        public MovieListViewModel MovieList { get; }
        public SidePanelViewModel SidePanel { get; }

        public string FormTitle
            => this.MovieSeries.MovieSeries.Title?.Name
                ?? this.MovieSeries.MovieSeries.GetFirstEntry().GetTitle().Name;

        public bool CanSaveChanges
            => this.MovieSeries.AreChangesPresent &&
                !this.MovieSeries.HasErrors &&
                !this.MovieSeries.Titles.Any(t => t.HasErrors) &&
                !this.MovieSeries.OriginalTitles.Any(t => t.HasErrors);

        public bool CanCancelChanges
            => this.MovieSeries.AreChangesPresent;

        public bool CanSaveOrCancelChanges
            => this.CanSaveChanges || this.CanCancelChanges;

        public async Task SaveAsync()
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

            this.MovieSeries.WriteChanges();

            bool shouldAddToList = this.MovieSeries.MovieSeries.Id == default && this.MovieSeries.HasName;

            await this.dbService.SaveMovieSeriesAsync(this.MovieSeries.MovieSeries, titlesToDelete);

            (shouldAddToList
                ? this.MovieList.AddItem
                : (this.MovieSeries.HasName ? this.MovieList.UpdateItem : this.MovieList.DeleteItem))
                .ExecuteIfCan(this.MovieSeries.MovieSeries);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            base.OnPropertyChanged(nameof(this.CanSaveChanges));
            base.OnPropertyChanged(nameof(this.CanCancelChanges));
            base.OnPropertyChanged(nameof(this.CanSaveOrCancelChanges));
        }
    }
}
