using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Akavache;

using MovieList.ViewModels.Forms;

using ReactiveUI;

namespace MovieList.Views.Forms
{
    public abstract class MovieFormControlBase : ReactiveUserControl<MovieFormViewModel> { }

    public partial class MovieFormControl : MovieFormControlBase
    {
        public MovieFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.ViewModel.FormTitle
                    .BindTo(this, v => v.FormTitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.LoadPoster();

                this.BindCommands(disposables);
                this.BindCheckboxes(disposables);
                this.BindLinks(disposables);
                this.BindFields(disposables);

                this.AddValidation(disposables);
            });
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            const BooleanToVisibilityHint useHidden = BooleanToVisibilityHint.UseHidden;

            this.BindCommand(this.ViewModel, vm => vm.Save, v => v.SaveButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToFranchise, v => v.GoToFranchiseButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToFranchise.CanExecute
                .BindTo(this, v => v.GoToFranchiseButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToFranchise, v => v.GoToFranchiseArrowButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToFranchise.CanExecute
                .BindTo(this, v => v.GoToFranchiseArrowButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToNext, v => v.GoToNextButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToNext.CanExecute
                .BindTo(this, v => v.GoToNextButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToPrevious, v => v.GoToPreviousButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToPrevious.CanExecute
                .BindTo(this, v => v.GoToPreviousButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.CreateFranchise, v => v.CreateFranchiseButton)
                .DisposeWith(disposables);

            this.ViewModel.CreateFranchise.CanExecute
                .BindTo(this, v => v.CreateFranchiseButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                .DisposeWith(disposables);

            this.ViewModel.Delete.CanExecute
                .BindTo(this, v => v.DeleteButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddTitle, v => v.AddTitleButton)
                .DisposeWith(disposables);

            this.ViewModel.AddTitle.CanExecute
                .BindTo(this, v => v.AddTitleButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddOriginalTitle, v => v.AddOriginalTitleButton)
                .DisposeWith(disposables);

            this.ViewModel.AddOriginalTitle.CanExecute
                .BindTo(this, v => v.AddOriginalTitleButton.Visibility)
                .DisposeWith(disposables);

            Observable.CombineLatest(this.ViewModel.Save.CanExecute, this.ViewModel.Cancel.CanExecute)
                .AnyTrue()
                .BindTo(this, v => v.ActionPanel.Visibility)
                .DisposeWith(disposables);

            this.ViewModel.Save
                .Subscribe(_ => this.LoadPoster())
                .DisposeWith(disposables);
        }

        private void BindCheckboxes(CompositeDisposable disposables)
        {
            this.Bind(this.ViewModel, vm => vm.IsWatched, v => v.IsWatchedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsReleased, v => v.IsReleasedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.Year)
                .Select(year => Int32.TryParse(year, out int value) ? (int?)value : null)
                .WhereValueNotNull()
                .Select(year => year <= DateTime.Now.Year)
                .BindTo(this, v => v.IsWatchedCheckBox.IsEnabled)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.Year)
                .Select(year => Int32.TryParse(year, out int value) ? (int?)value : null)
                .WhereValueNotNull()
                .Select(year => year == DateTime.Now.Year)
                .BindTo(this, v => v.IsReleasedCheckBox.IsEnabled)
                .DisposeWith(disposables);
        }

        private void BindLinks(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.ImdbLink, v => v.ImdbLink.NavigateUri)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.ImdbLink)
                .Select(link => !String.IsNullOrWhiteSpace(link))
                .BindTo(this, v => v.ImdbLinkTextBlock.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.RottenTomatoesLink, v => v.RottenTomatoesLink.NavigateUri)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.RottenTomatoesLink)
                .Select(link => !String.IsNullOrWhiteSpace(link))
                .BindTo(this, v => v.RottenTomatoesLinkTextBlock.Visibility)
                .DisposeWith(disposables);
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Titles, v => v.Titles.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.OriginalTitles, v => v.OriginalTitles.ItemsSource)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Year, v => v.YearTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Kind, v => v.KindComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.ImdbLink, v => v.ImdbLinkTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.RottenTomatoesLink, v => v.RottenTomatoesLinkTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlTextBox.Text)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Kinds, v => v.KindComboBox.ItemsSource)
                .DisposeWith(disposables);
        }

        private void AddValidation(CompositeDisposable disposables)
        {
            this.YearTextBox.ValidateWith(this.ViewModel.YearRule)
                .DisposeWith(disposables);

            this.ImdbLinkTextBox.ValidateWith(this.ViewModel.ImdbLinkRule)
                .DisposeWith(disposables);

            this.RottenTomatoesLinkTextBox.ValidateWith(this.ViewModel.RottenTomatoesLinkRule)
                .DisposeWith(disposables);

            this.PosterUrlTextBox.ValidateWith(this.ViewModel.PosterUrlRule)
                .DisposeWith(disposables);
        }

        private void LoadPoster()
        {
            if (!String.IsNullOrEmpty(this.ViewModel.PosterUrl))
            {
                BlobCache.UserAccount.DownloadUrl(this.ViewModel.PosterUrl, TimeSpan.FromMinutes(5))
                    .Select(data => data.AsImage())
                    .WhereNotNull()
                    .ObserveOnDispatcher()
                    .BindTo(this, v => v.Poster.Source);
            } else
            {
                this.Poster.Source = null;
            }
        }
    }
}
