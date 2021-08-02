using System.Reactive.Disposables;

using Avalonia.ReactiveUI;

using Cineaste.Core.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace Cineaste.Views.Forms.Preferences
{
    public partial class KindFormControl : ReactiveUserControl<KindFormViewModel>
    {
        public KindFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.Name, v => v.NameTextBox.Text)
                    .DisposeWith(disposables);

                this.BindDefaultValidation(this.ViewModel, vm => vm.Name, v => v.NameErrorTextBlock.Text)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                    .DisposeWith(disposables);

                this.ViewModel!.Delete.CanExecute
                    .BindTo(this, v => v.DeleteButton.IsVisible)
                    .DisposeWith(disposables);

                this.BindButtonColors(disposables);
                this.BindTextColors(disposables);
            });
        }

        private void BindButtonColors(CompositeDisposable disposables)
        {
            this.Bind(this.ViewModel, vm => vm.ColorForWatchedMovie, v => v.WatchedMovieButton.Color)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.ColorForNotWatchedMovie, v => v.NotWatchedMovieButton.Color)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.ColorForNotReleasedMovie, v => v.NotReleasedMovieButton.Color)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.ColorForWatchedSeries, v => v.WatchedSeriesButton.Color)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.ColorForNotWatchedSeries, v => v.NotWatchedSeriesButton.Color)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.ColorForNotReleasedSeries, v => v.NotReleasedSeriesButton.Color)
                .DisposeWith(disposables);
        }

        private void BindTextColors(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.ColorForWatchedMovie, v => v.WatchedMovieTextBlock.Foreground)
                .DisposeWith(disposables);

            this.OneWayBind(
                this.ViewModel, vm => vm.ColorForNotWatchedMovie, v => v.NotWatchedMovieTextBlock.Foreground)
                .DisposeWith(disposables);

            this.OneWayBind(
                this.ViewModel, vm => vm.ColorForNotReleasedMovie, v => v.NotReleasedMovieTextBlock.Foreground)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.ColorForWatchedSeries, v => v.WatchedSeriesTextBlock.Foreground)
                .DisposeWith(disposables);

            this.OneWayBind(
                this.ViewModel, vm => vm.ColorForNotWatchedSeries, v => v.NotWatchedSeriesTextBlock.Foreground)
                .DisposeWith(disposables);

            this.OneWayBind(
                this.ViewModel, vm => vm.ColorForNotReleasedSeries, v => v.NotReleasedSeriesTextBlock.Foreground)
                .DisposeWith(disposables);
        }
    }
}
