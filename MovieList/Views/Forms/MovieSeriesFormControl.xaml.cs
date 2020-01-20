using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Akavache;
using DynamicData;
using DynamicData.Binding;
using MovieList.ViewModels.Forms;

using ReactiveUI;

namespace MovieList.Views.Forms
{
    public abstract class MovieSeriesFormControlBase : ReactiveUserControl<MovieSeriesFormViewModel> { }

    public partial class MovieSeriesFormControl : MovieSeriesFormControlBase
    {
        public MovieSeriesFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.WhenAnyObservable(v => v.ViewModel.FormTitle)
                    .BindTo(this, v => v.FormTitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.LoadPoster();

                this.BindCommands(disposables);
                this.BindCheckboxes(disposables);
                this.BindFields(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Entries, v => v.Entries.ItemsSource)
                    .DisposeWith(disposables);

                this.AddValidation(disposables);
            });
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            var boolToVisibility = new BooleanToVisibilityTypeConverter();
            const BooleanToVisibilityHint useHidden = BooleanToVisibilityHint.UseHidden;

            this.BindCommand(this.ViewModel, vm => vm.Save, v => v.SaveButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToMovieSeries, v => v.GoToMovieSeriesButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.GoToMovieSeries.CanExecute)
                .BindTo(this, v => v.GoToMovieSeriesButton.Visibility, useHidden, boolToVisibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToNext, v => v.GoToNextButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.GoToNext.CanExecute)
                .BindTo(this, v => v.GoToNextButton.Visibility, useHidden, boolToVisibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToPrevious, v => v.GoToPreviousButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.GoToPrevious.CanExecute)
                .BindTo(this, v => v.GoToPreviousButton.Visibility, useHidden, boolToVisibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.Delete.CanExecute)
                .BindTo(this, v => v.DeleteButton.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddTitle, v => v.AddTitleButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.AddTitle.CanExecute)
                .BindTo(this, v => v.AddTitleButton.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddOriginalTitle, v => v.AddOriginalTitleButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.AddOriginalTitle.CanExecute)
                .BindTo(this, v => v.AddOriginalTitleButton.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            Observable.CombineLatest(
                    this.WhenAnyObservable(v => v.ViewModel.Save.CanExecute),
                    this.WhenAnyObservable(v => v.ViewModel.Cancel.CanExecute))
                .AnyTrue()
                .BindTo(this, v => v.ActionPanel.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.Save)
                .Subscribe(_ => this.LoadPoster())
                .DisposeWith(disposables);
        }

        private void BindCheckboxes(CompositeDisposable disposables)
        {
            this.Bind(this.ViewModel, vm => vm.HasTitles, v => v.HasTitlesCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.ShowTitles, v => v.ShowInListCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsLooselyConnected, v => v.IsLooselyConnectedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.HasTitles)
                .BindTo(this, v => v.ShowInListCheckBox.IsEnabled)
                .DisposeWith(disposables);

            this.ViewModel.Entries.ToObservableChangeSet()
                .AutoRefresh(vm => vm.DisplayNumber)
                .ToCollection()
                .Select(vms => vms.All(vm => vm.DisplayNumber.HasValue))
                .BindTo(this, v => v.IsLooselyConnectedCheckBox.IsEnabled)
                .DisposeWith(disposables);
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Titles, v => v.Titles.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.OriginalTitles, v => v.OriginalTitles.ItemsSource)
                .DisposeWith(disposables);

            var boolToVisibility = new BooleanToVisibilityTypeConverter();

            this.WhenAnyValue(v => v.ViewModel.HasTitles)
                .BindTo(this, v => v.Titles.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.HasTitles)
                .BindTo(this, v => v.OriginalTitles.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.HasTitles)
                .BindTo(this, v => v.AddTitleButton.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.HasTitles)
                .BindTo(this, v => v.AddOriginalTitleButton.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlTextBox.Text)
                .DisposeWith(disposables);
        }

        private void AddValidation(CompositeDisposable disposables)
            => this.PosterUrlTextBox.ValidateWith(this.ViewModel.PosterUrlRule)
                .DisposeWith(disposables);

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
