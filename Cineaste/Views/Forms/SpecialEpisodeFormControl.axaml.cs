using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Akavache;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Forms;

using ReactiveUI;

namespace Cineaste.Views.Forms
{
    public partial class SpecialEpisodeFormControl : ReactiveUserControl<SpecialEpisodeFormViewModel>
    {
        public SpecialEpisodeFormControl()
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

                this.LoadPoster();

                this.BindCommands(disposables);
                this.BindCheckboxes(disposables);
                this.BindLink(disposables);
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

            this.ViewModel!.Save
                .Subscribe(_ => this.LoadPoster())
                .DisposeWith(disposables);
        }

        private void BindLink(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.RottenTomatoesLink, v => v.RottenTomatoesLinkButton.NavigateUri)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.RottenTomatoesLink)
                .Select(link => !String.IsNullOrWhiteSpace(link))
                .BindTo(this, v => v.RottenTomatoesLinkButton.IsVisible)
                .DisposeWith(disposables);
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Titles, v => v.Titles.Items)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.OriginalTitles, v => v.OriginalTitles.Items)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Channel, v => v.ChannelTextBox.Text)
                .DisposeWith(disposables);

            this.WhenAnyValue(
                v => v.ViewModel!.Year,
                v => v.ViewModel!.Month,
                (year, month) => new DateTimeOffset(new DateTime(year, month, 1)))
                .BindTo(this, v => v.ReleaseDatePicker.SelectedDate)
                .DisposeWith(disposables);

            this.ReleaseDatePicker.GetObservable(DatePicker.SelectedDateProperty)
                .WhereValueNotNull()
                .Select(date => date.Year)
                .BindTo(this, v => v.ViewModel!.Year)
                .DisposeWith(disposables);

            this.ReleaseDatePicker.GetObservable(DatePicker.SelectedDateProperty)
                .WhereValueNotNull()
                .Select(date => date.Month)
                .BindTo(this, v => v.ViewModel!.Month)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.RottenTomatoesLink, v => v.RottenTomatoesLinkTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlTextBox.Text)
                .DisposeWith(disposables);
        }

        private void BindCheckboxes(CompositeDisposable disposables)
        {
            this.Bind(this.ViewModel, vm => vm.IsWatched, v => v.IsWatchedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsReleased, v => v.IsReleasedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Year)
                .CombineLatest(this.WhenAnyValue(v => v.ViewModel!.Month), (year, month) => (Year: year, Month: month))
                .Select(values =>
                    values.Year < DateTime.Now.Year ||
                    values.Year == DateTime.Now.Year && values.Month <= DateTime.Now.Month)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, v => v.IsWatchedCheckBox.IsEnabled)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Year)
                .CombineLatest(this.WhenAnyValue(v => v.ViewModel!.Month), (year, month) => (Year: year, Month: month))
                .Select(values => values.Month == DateTime.Now.Month && values.Year == DateTime.Now.Year)
                .BindTo(this, v => v.IsReleasedCheckBox.IsEnabled)
                .DisposeWith(disposables);
        }

        private void AddValidation(CompositeDisposable disposables)
        {
            this.BindDefaultValidation(this.ViewModel, vm => vm.Channel, v => v.ChannelErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindDefaultValidation(this.ViewModel, vm => vm.Year, v => v.ReleaseDateErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindDefaultValidation(
                this.ViewModel, vm => vm.RottenTomatoesLink, v => v.RottenTomatoesLinkErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.BindDefaultValidation(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlErrorTextBlock.Text)
                .DisposeWith(disposables);
        }

        private void LoadPoster()
        {
            if (!String.IsNullOrEmpty(this.ViewModel!.PosterUrl))
            {
                BlobCache.UserAccount.DownloadUrl(this.ViewModel!.PosterUrl, TimeSpan.FromMinutes(5))
                    .Select(data => data.AsImage())
                    .WhereNotNull()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.Poster.Source);
            } else
            {
                this.Poster.Source = null;
            }
        }
    }
}
