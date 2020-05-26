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

                this.OneWayBind(
                        this.ViewModel,
                        vm => vm.Name,
                        v => v.NameTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        vm => vm.ColorForWatchedMovie,
                        v => v.WatchedMovieColorRectangle.Fill)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        vm => vm.ColorForNotWatchedMovie,
                        v => v.NotWatchedMovieColorRectangle.Fill)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        vm => vm.ColorForNotReleasedMovie,
                        v => v.NotReleasedMovieColorRectangle.Fill)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        vm => vm.ColorForWatchedSeries,
                        v => v.WatchedSeriesColorRectangle.Fill)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        vm => vm.ColorForNotWatchedSeries,
                        v => v.NotWatchedSeriesColorRectangle.Fill)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        vm => vm.ColorForNotReleasedSeries,
                        v => v.NotReleasedSeriesColorRectangle.Fill)
                    .DisposeWith(disposables);
            });
        }
    }
}
