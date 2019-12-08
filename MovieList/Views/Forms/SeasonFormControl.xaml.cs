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
    public abstract class SeasonFormControlBase : ReactiveUserControl<SeasonFormViewModel> { }

    public partial class SeasonFormControl : SeasonFormControlBase
    {
        public SeasonFormControl()
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
                this.BindFields(disposables);

                this.AddValidation(disposables);
            });
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel, vm => vm.GoToSeries, v => v.GoToSeriesButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                .DisposeWith(disposables);

            var boolToVisibility = new BooleanToVisibilityTypeConverter();

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

            this.WhenAnyObservable(v => v.ViewModel.Cancel.CanExecute)
                .BindTo(this, v => v.CancelButton.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Titles, v => v.Titles.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.OriginalTitles, v => v.OriginalTitles.ItemsSource)
                .DisposeWith(disposables);

            this.WatchStatusComboBox.Items.Add(Messages.SeasonNotWatched);
            this.WatchStatusComboBox.Items.Add(Messages.SeasonWatching);
            this.WatchStatusComboBox.Items.Add(Messages.SeasonWatched);
            this.WatchStatusComboBox.Items.Add(Messages.SeasonStoppedWatching);

            this.BindComboBox(
                    vm => vm.WatchStatus,
                    v => v.WatchStatusComboBox.SelectedItem,
                    new SeasonWatchStatusConverter())
                .DisposeWith(disposables);

            this.ReleaseStatusComboBox.Items.Add(Messages.SeasonNotStarted);
            this.ReleaseStatusComboBox.Items.Add(Messages.SeasonRunning);
            this.ReleaseStatusComboBox.Items.Add(Messages.SeasonFinished);

            this.BindComboBox(
                    vm => vm.ReleaseStatus,
                    v => v.ReleaseStatusComboBox.SelectedItem,
                    new SeasonReleaseStatusConverter())
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Channel, v => v.ChannelTextBox.Text)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Periods, v => v.Periods.ItemsSource)
                .DisposeWith(disposables);
        }

        private IDisposable BindComboBox<T>(
            Expression<Func<SeasonFormViewModel, T>> vmProperty,
            Expression<Func<SeasonFormControl, object>> viewProperty,
            IBindingTypeConverter converter)
            => this.Bind(this.ViewModel, vmProperty, viewProperty, null, converter, converter);

        private void AddValidation(CompositeDisposable disposables)
        {
            this.ChannelTextBox.ValidateWith(this.ViewModel.ChannelRule)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.PeriodsNonOverlapping)
                .Select(isValid => isValid ? String.Empty : Messages.ValidationPeriodsOverlap)
                .BindTo(this, v => v.InvalidFormTextBlock.Text)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.PeriodsNonOverlapping)
                .BindTo(this, v => v.InvalidFormTextBlock.Visibility, null, new BooleanToVisibilityTypeConverter())
                .DisposeWith(disposables);
        }

        private void LoadPoster()
        {
            if (!String.IsNullOrEmpty(this.ViewModel.CurrentPosterUrl))
            {
                BlobCache.UserAccount.DownloadUrl(this.ViewModel.CurrentPosterUrl, TimeSpan.FromMinutes(5))
                    .Select(data => data.AsImage())
                    .WhereNotNull()
                    .ObserveOnDispatcher()
                    .BindTo(this, v => v.Poster.Source);
            }
        }
    }
}
