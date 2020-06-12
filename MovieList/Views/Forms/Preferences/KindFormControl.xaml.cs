using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

using MovieList.DialogModels;
using MovieList.Properties;
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

                this.NameTextBox.ValidateWith(this.ViewModel.NameRule)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                    .DisposeWith(disposables);

                this.ViewModel.Delete.CanExecute
                    .BindTo(this, v => v.DeleteButton.Visibility)
                    .DisposeWith(disposables);

                this.BindColors(disposables);
                this.EnablePickingNewColors(disposables);
            });
        }

        private void BindColors(CompositeDisposable disposables)
        {
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
        }

        private void EnablePickingNewColors(CompositeDisposable disposables)
        {
            this.PickNewColorOnClick(this.WatchedMovieColorRectangle, vm => vm.ColorForWatchedMovie)
                .DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotWatchedMovieColorRectangle, vm => vm.ColorForNotWatchedMovie)
                .DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotReleasedMovieColorRectangle, vm => vm.ColorForNotReleasedMovie)
                .DisposeWith(disposables);

            this.PickNewColorOnClick(this.WatchedSeriesColorRectangle, vm => vm.ColorForWatchedSeries)
                .DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotWatchedSeriesColorRectangle, vm => vm.ColorForNotWatchedSeries)
                .DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotReleasedSeriesColorRectangle, vm => vm.ColorForNotReleasedSeries)
                .DisposeWith(disposables);
        }

        private IDisposable PickNewColorOnClick(
            FrameworkElement element,
            Expression<Func<KindFormViewModel, string>> colorProperty)
        {
            var getColor = colorProperty.Compile();
            return element.Events().MouseLeftButtonUp
                .SelectMany(_ => this.PickNewColor(getColor(this.ViewModel)))
                .WhereNotNull()
                .ObserveOnDispatcher()
                .BindTo(this.ViewModel, colorProperty);
        }

        private IObservable<string?> PickNewColor(string color)
            => Dialog.ColorPicker.Handle(
                new ColorModel(nameof(Messages.ColorPickerMessage), nameof(Messages.ColorPickerTitle))
                {
                    Color = color
                });
    }
}
