using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Akavache;

using MovieList.Converters;
using MovieList.Properties;
using MovieList.ViewModels.Forms;

using ReactiveUI;

namespace MovieList.Views.Forms
{
    public abstract class SeriesFormControlBase : ReactiveUserControl<SeriesFormViewModel> { }

    public partial class SeriesFormControl : SeriesFormControlBase
    {
        public SeriesFormControl()
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
                this.BindLink(disposables);
                this.BindFields(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Components, v => v.Components.ItemsSource)
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

            this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToMovieSeries, v => v.GoToMovieSeriesButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.GoToMovieSeries.CanExecute)
                .BindTo(this, v => v.GoToMovieSeriesButton.Visibility, useHidden, boolToVisibility)
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
                .BindTo(this, v => v.AddOriginalTitleButton.Visibility, null, boolToVisibility);

            this.BindCommand(this.ViewModel, vm => vm.AddSeason, v => v.AddSeasonButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddSpecialEpisode, v => v.AddSpecialEpisodeButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.ConvertToMiniseries, v => v.ConvertToMiniseriesButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.ConvertToMiniseries.CanExecute)
                .BindTo(this, v => v.ConvertToMiniseriesButton.Visibility, null, boolToVisibility)
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

        private void BindLink(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.ImdbLink, v => v.ImdbLink.NavigateUri)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.ImdbLink, v => v.ImdbLinkRun.Text)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.ImdbLink)
                .Select(link => !String.IsNullOrWhiteSpace(link))
                .BindTo(this, v => v.ImdbLinkTextBlock.Visibility, null, new BooleanToVisibilityTypeConverter())
                .DisposeWith(disposables);
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Titles, v => v.Titles.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.OriginalTitles, v => v.OriginalTitles.ItemsSource)
                .DisposeWith(disposables);

            this.WatchStatusComboBox.Items.Add(Messages.SeriesNotWatched);
            this.WatchStatusComboBox.Items.Add(Messages.SeriesWatching);
            this.WatchStatusComboBox.Items.Add(Messages.SeriesWatched);
            this.WatchStatusComboBox.Items.Add(Messages.SeriesStoppedWatching);

            this.BindComboBox(
                    vm => vm.WatchStatus,
                    v => v.WatchStatusComboBox.SelectedItem,
                    new SeriesWatchStatusConverter())
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsAnthology, v => v.IsAnthologyCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.ReleaseStatusComboBox.Items.Add(Messages.SeriesNotStarted);
            this.ReleaseStatusComboBox.Items.Add(Messages.SeriesRunning);
            this.ReleaseStatusComboBox.Items.Add(Messages.SeriesFinished);
            this.ReleaseStatusComboBox.Items.Add(Messages.SeriesCancelled);

            this.BindComboBox(
                    vm => vm.ReleaseStatus,
                    v => v.ReleaseStatusComboBox.SelectedItem,
                    new SeriesReleaseStatusConverter())
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Kind, v => v.KindComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.ImdbLink, v => v.ImdbLinkTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlTextBox.Text)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Kinds, v => v.KindComboBox.ItemsSource)
                .DisposeWith(disposables);
        }

        private IDisposable BindComboBox<T>(
            Expression<Func<SeriesFormViewModel, T>> vmProperty,
            Expression<Func<SeriesFormControl, object>> viewProperty,
            IBindingTypeConverter converter)
            => this.Bind(this.ViewModel, vmProperty, viewProperty, null, converter, converter);

        private void AddValidation(CompositeDisposable disposables)
        {
            this.ImdbLinkTextBox.ValidateWith(this.ViewModel.ImdbLinkRule)
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
