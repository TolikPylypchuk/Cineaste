using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

            this.OpenForm = new DelegateCommand<MovieSeriesComponentFormItemBase>(c => c.OpenForm(this.SidePanel));
            this.ShowOrdinalNumber = new DelegateCommand<MovieSeriesComponentFormItemBase>(
                this.OnShowOrdinalNumber, this.CanShowOrdinalNumber);
            this.HideOrdinalNumber = new DelegateCommand<MovieSeriesComponentFormItemBase>(
                this.OnHideOrdinalNumber, this.CanHideOrdinalNumber);
            this.MoveComponentUp = new DelegateCommand<MovieSeriesComponentFormItemBase>(
                this.OnMoveComponentUp, this.CanMoveComponentUp);
            this.MoveComponentDown = new DelegateCommand<MovieSeriesComponentFormItemBase>(
                this.OnMoveComponentDown, this.CanMoveComponentDown);
            this.DetachComponent = new DelegateCommand<MovieSeriesComponentFormItemBase>(this.OnDetachComponent);

            this.AttachMovie = new DelegateCommand<Movie>(this.OnAttachMovie);
            this.AttachSeries = new DelegateCommand<Series>(this.OnAttachSeries);
            this.AttachMovieSeries = new DelegateCommand<MovieSeries>(this.OnAttachMovieSeries);
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

        public DelegateCommand Save { get; }
        public DelegateCommand Cancel { get; }

        public DelegateCommand<MovieSeriesComponentFormItemBase> OpenForm { get; }
        public DelegateCommand<MovieSeriesComponentFormItemBase> ShowOrdinalNumber { get; }
        public DelegateCommand<MovieSeriesComponentFormItemBase> HideOrdinalNumber { get; }
        public DelegateCommand<MovieSeriesComponentFormItemBase> MoveComponentUp { get; }
        public DelegateCommand<MovieSeriesComponentFormItemBase> MoveComponentDown { get; }
        public DelegateCommand<MovieSeriesComponentFormItemBase> DetachComponent { get; }

        public DelegateCommand<Movie> AttachMovie { get; }
        public DelegateCommand<Series> AttachSeries { get; }
        public DelegateCommand<MovieSeries> AttachMovieSeries { get; }

        public MovieSeriesFormControl MovieSeriesFormControl { get; set; }

        public MovieListViewModel MovieList { get; }
        public SidePanelViewModel SidePanel { get; }

        public string FormTitle
            => this.MovieSeries.MovieSeries.GetTitleName();

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

        private void OnShowOrdinalNumber(MovieSeriesComponentFormItemBase component)
        {
        }

        private bool CanShowOrdinalNumber(MovieSeriesComponentFormItemBase component)
            => component switch
            {
                MovieSeriesEntryFormItemBase entry => entry.MovieSeriesEntry?.DisplayNumber == null,
                MovieSeriesFormItem movieSeries => movieSeries.DisplayNumber == null,
                _ => false
            };

        private void OnHideOrdinalNumber(MovieSeriesComponentFormItemBase component)
        {
        }

        private bool CanHideOrdinalNumber(MovieSeriesComponentFormItemBase component)
            => component switch
            {
                MovieSeriesEntryFormItemBase entry => entry.MovieSeriesEntry?.DisplayNumber != null,
                MovieSeriesFormItem movieSeries => movieSeries.DisplayNumber != null,
                _ => false
            };

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

        private void OnAttachMovie(Movie movie)
        {
        }

        private void OnAttachSeries(Series series)
        {
        }

        private void OnAttachMovieSeries(MovieSeries movieSeries)
        {
        }
    }
}
