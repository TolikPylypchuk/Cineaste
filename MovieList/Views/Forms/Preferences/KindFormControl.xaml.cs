using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

using MovieList.Core;
using MovieList.Core.DialogModels;
using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Properties;

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
                    ?.DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.Name, v => v.NameTextBox.Text)
                    ?.DisposeWith(disposables);

                this.NameTextBox.ValidateWith(this.ViewModel!.NameRule)
                    ?.DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                    ?.DisposeWith(disposables);

                this.ViewModel.Delete.CanExecute
                    .BindTo(this, v => v.DeleteButton.Visibility)
                    ?.DisposeWith(disposables);

                this.BindChipIconColors(disposables);
                this.BindChipTextColors(disposables);
                this.EnablePickingNewColors(disposables);
            });
        }

        private void BindChipIconColors(CompositeDisposable disposables)
        {
            this.OneWayBind(
                this.ViewModel,
                vm => vm.ColorForWatchedMovie,
                v => v.WatchedMovieChip.IconBackground)
                ?.DisposeWith(disposables);

            this.OneWayBind(
                this.ViewModel,
                vm => vm.ColorForNotWatchedMovie,
                v => v.NotWatchedMovieChip.IconBackground)
                ?.DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForNotReleasedMovie,
                    v => v.NotReleasedMovieChip.IconBackground)
                ?.DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForWatchedSeries,
                    v => v.WatchedSeriesChip.IconBackground)
                ?.DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForNotWatchedSeries,
                    v => v.NotWatchedSeriesChip.IconBackground)
                ?.DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForNotReleasedSeries,
                    v => v.NotReleasedSeriesChip.IconBackground)
                ?.DisposeWith(disposables);
        }

        private void BindChipTextColors(CompositeDisposable disposables)
        {
            this.OneWayBind(
                this.ViewModel,
                vm => vm.ColorForWatchedMovie,
                v => v.WatchedMovieChip.Foreground)
                ?.DisposeWith(disposables);

            this.OneWayBind(
                this.ViewModel,
                vm => vm.ColorForNotWatchedMovie,
                v => v.NotWatchedMovieChip.Foreground)
                ?.DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForNotReleasedMovie,
                    v => v.NotReleasedMovieChip.Foreground)
                ?.DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForWatchedSeries,
                    v => v.WatchedSeriesChip.Foreground)
                ?.DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForNotWatchedSeries,
                    v => v.NotWatchedSeriesChip.Foreground)
                ?.DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForNotReleasedSeries,
                    v => v.NotReleasedSeriesChip.Foreground)
                ?.DisposeWith(disposables);
        }

        private void EnablePickingNewColors(CompositeDisposable disposables)
        {
            this.PickNewColorOnClick(this.WatchedMovieChip, vm => vm.ColorForWatchedMovie)
                ?.DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotWatchedMovieChip, vm => vm.ColorForNotWatchedMovie)
                ?.DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotReleasedMovieChip, vm => vm.ColorForNotReleasedMovie)
                ?.DisposeWith(disposables);

            this.PickNewColorOnClick(this.WatchedSeriesChip, vm => vm.ColorForWatchedSeries)
                ?.DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotWatchedSeriesChip, vm => vm.ColorForNotWatchedSeries)
                ?.DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotReleasedSeriesChip, vm => vm.ColorForNotReleasedSeries)
                ?.DisposeWith(disposables);
        }

        private IDisposable PickNewColorOnClick(
            ButtonBase element,
            Expression<Func<KindFormViewModel, string>> colorProperty)
        {
            var getColor = colorProperty.Compile();
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => element.Click += h, h => element.Click -= h)
                .SelectMany(_ => this.PickNewColor(getColor(this.ViewModel!)))
                .WhereNotNull()
                .ObserveOnDispatcher()
                .BindTo(this.ViewModel!, colorProperty);
        }

        private IObservable<string?> PickNewColor(string color)
            => Dialog.ColorPicker.Handle(
                new ColorModel(nameof(Messages.ColorPickerMessage), nameof(Messages.ColorPickerTitle))
                {
                    Color = color
                });
    }
}
