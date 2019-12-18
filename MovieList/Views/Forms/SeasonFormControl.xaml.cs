using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Akavache;

using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;

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
            var boolToVisibility = new BooleanToVisibilityTypeConverter();

            this.BindCommand(this.ViewModel, vm => vm.GoToSeries, v => v.GoToSeriesButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.Cancel.CanExecute)
                .BindTo(this, v => v.CancelButton.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.SelectNext, v => v.SelectNextButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.SelectNext.CanExecute)
                .BindTo(this, v => v.SelectNextButton.Visibility, BooleanToVisibilityHint.UseHidden, boolToVisibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.SelectPrevious, v => v.SelectPreviousButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.SelectPrevious.CanExecute)
                .BindTo(this, v => v.SelectPreviousButton.Visibility, BooleanToVisibilityHint.UseHidden, boolToVisibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.Delete.CanExecute)
                .BindTo(this, v => v.DeleteButton.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.SwitchToNextPoster, v => v.SwitchToNextPosterButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.SwitchToNextPoster.CanExecute)
                .BindTo(this, v => v.SwitchToNextPosterButton.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.SwitchToPreviousPoster, v => v.SwitchToPreviousPosterButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.SwitchToPreviousPoster.CanExecute)
                .BindTo(this, v => v.SwitchToPreviousPosterButton.Visibility, null, boolToVisibility)
                .DisposeWith(disposables);

            this.ViewModel.Periods.ToObservableChangeSet()
                .Count()
                .Select(count => count > 1)
                .ObserveOnDispatcher()
                .BindTo(this, v => v.SwitchPosterPanel.Visibility, null, boolToVisibility)
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

            this.BindCommand(this.ViewModel, vm => vm.AddPeriod, v => v.AddPeriodButton)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.AddPeriod.CanExecute)
                .BindTo(this, v => v.AddPeriodButton.Visibility, null, boolToVisibility)
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

            var allPeriodsValid = this.ViewModel.Periods.ToObservableChangeSet()
                .AutoRefreshOnObservable(period => period.Valid)
                .ToCollection()
                .Select(periods => periods.All(period => !period.HasErrors));

            this.WhenAnyObservable(v => v.ViewModel.PeriodsNonOverlapping)
                .CombineLatest(allPeriodsValid, (a, b) => !a && b)
                .ObserveOnDispatcher()
                .BindTo(this, v => v.PeriodsOverlapTextBlock.Visibility, null, new BooleanToVisibilityTypeConverter())
                .DisposeWith(disposables);
        }

        private void LoadPoster()
        {
            this.WhenAnyValue(v => v.ViewModel.CurrentPosterUrl)
                .Where(url => !String.IsNullOrEmpty(url))
                .SelectMany(url => BlobCache.UserAccount.DownloadUrl(url, TimeSpan.FromMinutes(5)))
                .Select(data => data.AsImage())
                .WhereNotNull()
                .ObserveOnDispatcher()
                .BindTo(this, v => v.Poster.Source);
        }
    }
}
