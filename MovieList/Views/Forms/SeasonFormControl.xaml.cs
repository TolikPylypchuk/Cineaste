using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;

using Akavache;

using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;

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

                this.ViewModel.FormTitle
                    .BindTo(this, v => v.FormTitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.LoadPoster(disposables);

                this.BindCommands(disposables);
                this.BindFields(disposables);

                this.AddValidation(disposables);
            });
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            const BooleanToVisibilityHint useHidden = BooleanToVisibilityHint.UseHidden;

            this.BindCommand(this.ViewModel, vm => vm.GoToSeries, v => v.GoToSeriesButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToSeries, v => v.GoToSeriesIconButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToSeries.CanExecute
                .BindTo(this, v => v.GoToSeriesIconButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            this.ViewModel.Cancel.CanExecute
                .BindTo(this, v => v.CancelButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
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

            this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                .DisposeWith(disposables);

            this.ViewModel.Delete.CanExecute
                .BindTo(this, v => v.DeleteButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.SwitchToNextPoster, v => v.SwitchToNextPosterButton)
                .DisposeWith(disposables);

            this.ViewModel.SwitchToNextPoster.CanExecute
                .BindTo(this, v => v.SwitchToNextPosterButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.SwitchToPreviousPoster, v => v.SwitchToPreviousPosterButton)
                .DisposeWith(disposables);

            this.ViewModel.SwitchToPreviousPoster.CanExecute
                .BindTo(this, v => v.SwitchToPreviousPosterButton.Visibility)
                .DisposeWith(disposables);

            this.ViewModel.Periods.ToObservableChangeSet()
                .Count()
                .Select(count => count > 1)
                .ObserveOnDispatcher()
                .BindTo(this, v => v.SwitchPosterPanel.Visibility)
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

            this.BindCommand(this.ViewModel, vm => vm.AddPeriod, v => v.AddPeriodButton)
                .DisposeWith(disposables);

            this.ViewModel.AddPeriod.CanExecute
                .BindTo(this, v => v.AddPeriodButton.Visibility)
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
            this.WatchStatusComboBox.Items.Add(Messages.SeasonHiatus);
            this.WatchStatusComboBox.Items.Add(Messages.SeasonWatched);
            this.WatchStatusComboBox.Items.Add(Messages.SeasonStoppedWatching);

            this.Bind(this.ViewModel, vm => vm.WatchStatus, v => v.WatchStatusComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.ReleaseStatusComboBox.Items.Add(Messages.SeasonNotStarted);
            this.ReleaseStatusComboBox.Items.Add(Messages.SeasonRunning);
            this.ReleaseStatusComboBox.Items.Add(Messages.SeasonHiatus);
            this.ReleaseStatusComboBox.Items.Add(Messages.SeasonFinished);
            this.ReleaseStatusComboBox.Items.Add(Messages.SeasonUnknown);

            this.Bind(this.ViewModel, vm => vm.ReleaseStatus, v => v.ReleaseStatusComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Channel, v => v.ChannelTextBox.Text)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Periods, v => v.Periods.ItemsSource)
                .DisposeWith(disposables);
        }

        private void AddValidation(CompositeDisposable disposables)
        {
            this.ChannelTextBox.ValidateWith(this.ViewModel.ChannelRule)
                .DisposeWith(disposables);

            var allPeriodsValid = this.ViewModel.Periods.ToObservableChangeSet()
                .AutoRefreshOnObservable(period => period.Valid)
                .ToCollection()
                .Select(periods => periods.All(period => !period.HasErrors));

            this.ViewModel.PeriodsNonOverlapping
                .CombineLatest(allPeriodsValid, (a, b) => !a && b)
                .ObserveOnDispatcher()
                .BindTo(this, v => v.PeriodsOverlapTextBlock.Visibility)
                .DisposeWith(disposables);
        }

        private void LoadPoster(CompositeDisposable disposables)
        {
            this.WhenAnyValue(v => v.ViewModel.CurrentPosterUrl)
                .Where(url => !String.IsNullOrEmpty(url))
                .SelectMany(url => BlobCache.UserAccount.DownloadUrl(url, TimeSpan.FromMinutes(5)))
                .Select(data => data.AsImage())
                .WhereNotNull()
                .ObserveOnDispatcher()
                .BindTo(this, v => v.Poster.Source)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.CurrentPosterUrl)
                .Where(String.IsNullOrEmpty)
                .Select<string?, ImageSource?>(_ => null)
                .BindTo(this, v => v.Poster.Source)
                .DisposeWith(disposables);
        }
    }
}
