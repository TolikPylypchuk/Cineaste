using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Akavache;

using Avalonia.Media;
using Avalonia.ReactiveUI;

using Cineaste.Core.ViewModels.Forms;
using Cineaste.Data.Models;

using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;

using ReactiveUI;

namespace Cineaste.Views.Forms
{
    public partial class SeasonFormControl : ReactiveUserControl<SeasonFormViewModel>
    {
        public SeasonFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.ViewModel!.FormTitle
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
            this.BindCommand(this.ViewModel!, vm => vm.GoToSeries, v => v.GoToSeriesButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.GoToSeries, v => v.GoToSeriesIconButton)
                .DisposeWith(disposables);

            this.ViewModel!.GoToSeries.CanExecute
                .BindTo(this, v => v.GoToSeriesIconButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            this.ViewModel!.Cancel.CanExecute
                .BindTo(this, v => v.CancelButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.GoToNext, v => v.GoToNextButton)
                .DisposeWith(disposables);

            this.ViewModel!.GoToNext.CanExecute
                .BindTo(this, v => v.GoToNextButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.GoToPrevious, v => v.GoToPreviousButton)
                .DisposeWith(disposables);

            this.ViewModel!.GoToPrevious.CanExecute
                .BindTo(this, v => v.GoToPreviousButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Delete, v => v.DeleteButton)
                .DisposeWith(disposables);

            this.ViewModel!.Delete.CanExecute
                .BindTo(this, v => v.DeleteButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.SwitchToNextPoster, v => v.SwitchToNextPosterButton)
                .DisposeWith(disposables);

            this.ViewModel!.SwitchToNextPoster.CanExecute
                .BindTo(this, v => v.SwitchToNextPosterButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.SwitchToPreviousPoster, v => v.SwitchToPreviousPosterButton)
                .DisposeWith(disposables);

            this.ViewModel!.SwitchToPreviousPoster.CanExecute
                .BindTo(this, v => v.SwitchToPreviousPosterButton.IsVisible)
                .DisposeWith(disposables);

            this.ViewModel!.Periods.ToObservableChangeSet()
                .Count()
                .Select(count => count > 1)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, v => v.SwitchPosterPanel.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.AddTitle, v => v.AddTitleButton)
                .DisposeWith(disposables);

            this.ViewModel!.AddTitle.CanExecute
                .BindTo(this, v => v.AddTitleButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.AddOriginalTitle, v => v.AddOriginalTitleButton)
                .DisposeWith(disposables);

            this.ViewModel!.AddOriginalTitle.CanExecute
                .BindTo(this, v => v.AddOriginalTitleButton.IsVisible)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.AddPeriod, v => v.AddPeriodButton)
                .DisposeWith(disposables);

            this.ViewModel!.AddPeriod.CanExecute
                .BindTo(this, v => v.AddPeriodButton.IsVisible)
                .DisposeWith(disposables);
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Titles, v => v.Titles.Items)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.OriginalTitles, v => v.OriginalTitles.Items)
                .DisposeWith(disposables);

            this.WatchStatusComboBox.SetEnumValues<SeasonWatchStatus>();

            this.Bind(this.ViewModel, vm => vm.WatchStatus, v => v.WatchStatusComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.ReleaseStatusComboBox.SetEnumValues<SeasonReleaseStatus>();

            this.Bind(this.ViewModel, vm => vm.ReleaseStatus, v => v.ReleaseStatusComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Channel, v => v.ChannelTextBox.Text)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Periods, v => v.Periods.Items)
                .DisposeWith(disposables);
        }

        private void AddValidation(CompositeDisposable disposables)
        {
            this.BindStrictValidation(
                this.ViewModel, vm => vm.Channel, v => v.ChannelErrorTextBlock.Text, magicallyWorks: false)
                .DisposeWith(disposables);

            var allPeriodsValid = this.ViewModel!.Periods
                .ToObservableChangeSet()
                .AutoRefreshOnObservable(period => period.Valid)
                .ToCollection()
                .Select(periods => periods.All(period => !period.HasErrors));

            this.ViewModel.PeriodsNonOverlapping
                .CombineLatest(allPeriodsValid, (a, b) => !a && b)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, v => v.PeriodsOverlapTextBlock.IsVisible)
                .DisposeWith(disposables);
        }

        private void LoadPoster(CompositeDisposable disposables)
        {
            this.WhenAnyValue(v => v.ViewModel!.CurrentPosterUrl)
                .Where(url => !String.IsNullOrEmpty(url))
                .WhereNotNull()
                .SelectMany(url => BlobCache.UserAccount.DownloadUrl(url, TimeSpan.FromMinutes(5)))
                .Select(data => data.AsImage())
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, v => v.Poster.Source)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.CurrentPosterUrl)
                .Where(String.IsNullOrEmpty)
                .Select<string?, IImage?>(_ => null)
                .BindTo(this, v => v.Poster.Source)
                .DisposeWith(disposables);
        }
    }
}
