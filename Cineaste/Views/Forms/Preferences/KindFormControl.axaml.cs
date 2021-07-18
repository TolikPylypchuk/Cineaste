using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Controls;
using Cineaste.Core;
using Cineaste.Core.DialogModels;
using Cineaste.Core.ViewModels.Forms.Preferences;
using Cineaste.Properties;

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

                this.BindChipIconColors(disposables);
                this.BindChipTextColors(disposables);
                this.EnablePickingNewColors(disposables);
            });
        }

        private void BindChipIconColors(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.ColorForWatchedMovie, v => v.WatchedMovieChip.TagBrush)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.ColorForNotWatchedMovie, v => v.NotWatchedMovieChip.TagBrush)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.ColorForNotReleasedMovie, v => v.NotReleasedMovieChip.TagBrush)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.ColorForWatchedSeries, v => v.WatchedSeriesChip.TagBrush)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.ColorForNotWatchedSeries, v => v.NotWatchedSeriesChip.TagBrush)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.ColorForNotReleasedSeries, v => v.NotReleasedSeriesChip.TagBrush)
                .DisposeWith(disposables);
        }

        private void BindChipTextColors(CompositeDisposable disposables)
        {
            this.OneWayBind(
                this.ViewModel,
                vm => vm.ColorForWatchedMovie,
                v => v.WatchedMovieChip.Foreground)
                .DisposeWith(disposables);

            this.OneWayBind(
                this.ViewModel,
                vm => vm.ColorForNotWatchedMovie,
                v => v.NotWatchedMovieChip.Foreground)
                .DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForNotReleasedMovie,
                    v => v.NotReleasedMovieChip.Foreground)
                .DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForWatchedSeries,
                    v => v.WatchedSeriesChip.Foreground)
                .DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForNotWatchedSeries,
                    v => v.NotWatchedSeriesChip.Foreground)
                .DisposeWith(disposables);

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.ColorForNotReleasedSeries,
                    v => v.NotReleasedSeriesChip.Foreground)
                .DisposeWith(disposables);
        }

        private void EnablePickingNewColors(CompositeDisposable disposables)
        {
            this.PickNewColorOnClick(this.WatchedMovieChip, vm => vm.ColorForWatchedMovie)
                .DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotWatchedMovieChip, vm => vm.ColorForNotWatchedMovie)
                .DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotReleasedMovieChip, vm => vm.ColorForNotReleasedMovie)
                .DisposeWith(disposables);

            this.PickNewColorOnClick(this.WatchedSeriesChip, vm => vm.ColorForWatchedSeries)
                .DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotWatchedSeriesChip, vm => vm.ColorForNotWatchedSeries)
                .DisposeWith(disposables);

            this.PickNewColorOnClick(this.NotReleasedSeriesChip, vm => vm.ColorForNotReleasedSeries)
                .DisposeWith(disposables);
        }

        private IDisposable PickNewColorOnClick(Chip chip, Expression<Func<KindFormViewModel, string>> colorProperty)
        {
            var getColor = colorProperty.Compile();
            return chip.GetObservable(Chip.ClickEvent)
                .SelectMany(_ => this.PickNewColor(getColor(this.ViewModel!)))
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this.ViewModel!, colorProperty!);
        }

        private IObservable<string?> PickNewColor(string color)
            => Dialog.ColorPicker.Handle(
                new ColorModel(nameof(Messages.ColorPickerMessage), nameof(Messages.ColorPickerTitle))
                {
                    Color = color
                });
    }
}
