using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Media;

using MovieList.Core;
using MovieList.Core.ListItems;
using MovieList.Core.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class ListItemControlBase : ReactiveUserControl<ListItemViewModel> { }

    public partial class ListItemControl : ListItemControlBase
    {
        private static readonly SolidColorBrush PartialHighlightBrush =
            new(new Color { A = 255, R = 237, G = 231, B = 246 });

        private static readonly SolidColorBrush FullHighlightBrush =
            new(new Color { A = 255, R = 209, G = 196, B = 233 });

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

                var isMouseOver = this.Events().MouseEnter.Select(_ => true)
                    .Merge(this.Events().MouseLeave.Select(_ => false))
                    .DistinctUntilChanged()
                    .StartWith(false);

                this.WhenAnyValue(
                    v => v.ViewModel!.Item.HighlightMode,
                    v => v.ViewModel!.Item.IsSelected,
                    (highlightMode, selected) => (Mode: highlightMode, Selected: selected))
                    .CombineLatest(isMouseOver, (item, mouseOver) => (item.Mode, Show: !item.Selected && !mouseOver))
                    .Select(item => item.Show ? GetBrushForHighlightMode(item.Mode) : Brushes.Transparent)
                    .ObserveOnDispatcher()
                    .BindTo(this, v => v.Background)
                    .DisposeWith(disposables);

                this.OneWayBind(
                    this.ViewModel,
                    vm => vm.Item.Color,
                    v => v.Foreground,
                    color => new SolidColorBrush
                    {
                        Color = (Color?)ColorConverter.ConvertFromString(color) ?? Colors.Black
                    })
                    .DisposeWith(disposables);
            });
        }

        private Brush GetBrushForHighlightMode(HighlightMode mode) =>
            mode switch
            {
                HighlightMode.Partial => PartialHighlightBrush,
                HighlightMode.Full => FullHighlightBrush,
                _ => Brushes.Transparent
            };
    }
}
