using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.ReactiveUI;

using Cineaste.Core.ListItems;
using Cineaste.Core.ViewModels;

using ReactiveUI;

namespace Cineaste.Views
{
    public partial class ListItemControl : ReactiveUserControl<ListItemViewModel>
    {
        public ListItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.DisplayNumber, v => v.NumberTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.Title, v => v.TitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.OriginalTitle, v => v.OriginalTitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.Year, v => v.YearTextBlock.Text)
                    .DisposeWith(disposables);

                var isMouseOver = this.GetObservable(PointerEnterEvent).Select(_ => true)
                    .Merge(this.GetObservable(PointerLeaveEvent).Select(_ => false))
                    .DistinctUntilChanged()
                    .StartWith(false);

                this.WhenAnyValue(
                    v => v.ViewModel!.Item.HighlightMode,
                    v => v.ViewModel!.Item.IsSelected,
                    (highlightMode, selected) => (Mode: highlightMode, Selected: selected))
                    .CombineLatest(isMouseOver, (item, mouseOver) => (item.Mode, Show: !item.Selected && !mouseOver))
                    .Select(item => item.Show ? this.GetColorForHighlightMode(item.Mode) : Brushes.Transparent)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.Background)
                    .DisposeWith(disposables);

                this.OneWayBind(
                    this.ViewModel,
                    vm => vm.Color,
                    v => v.Foreground,
                    color => Color.TryParse(color, out var result) ? result.ToBrush() : Brushes.Black)
                    .DisposeWith(disposables);
            });
        }

        private IBrush GetColorForHighlightMode(HighlightMode mode) =>
            mode switch
            {
                HighlightMode.Partial =>
                    this.TryFindResource("AccentAcrylicBackgroundFillColorDefaultBrush", out object? resource) &&
                    resource is IBrush brush
                    ? brush
                    : Brushes.Transparent,
                HighlightMode.Full =>
                    this.TryFindResource("AccentAcrylicBackgroundFillColorBaseBrush", out object? resource) &&
                    resource is IBrush brush
                    ? brush
                    : Brushes.Transparent,
                _ => Brushes.Transparent
            };
    }
}
