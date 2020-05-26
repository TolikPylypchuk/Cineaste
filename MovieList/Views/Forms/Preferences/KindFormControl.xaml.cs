using System.Reactive.Disposables;

using MovieList.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace MovieList.Views.Forms.Preferences
{
    public abstract class KindFormControlBase : ReactiveUserControl<KindFormViewModel> { }

    public partial class KindFormControl : KindFormControlBase
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

                this.Bind(
                        this.ViewModel,
                        vm => vm.ColorForWatchedMovie,
                        v => v.WatchedMovieColorPicker.ColorTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        vm => vm.ColorForNotWatchedMovie,
                        v => v.NotWatchedMovieColorPicker.ColorTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        vm => vm.ColorForNotReleasedMovie,
                        v => v.NotReleasedMovieColorPicker.ColorTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        vm => vm.ColorForWatchedSeries,
                        v => v.WatchedSeriesColorPicker.ColorTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        vm => vm.ColorForNotWatchedSeries,
                        v => v.NotWatchedSeriesColorPicker.ColorTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        vm => vm.ColorForNotReleasedSeries,
                        v => v.NotReleasedSeriesColorPicker.ColorTextBox.Text)
                    .DisposeWith(disposables);

                this.WatchedMovieColorPicker.ColorTextBox.ValidateWith(this.ViewModel.ColorForNotWatchedMovieRule)
                    .DisposeWith(disposables);
            });
        }
    }
}
