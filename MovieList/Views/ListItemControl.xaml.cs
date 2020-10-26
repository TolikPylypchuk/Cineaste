using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Controls;
using System.Windows.Media;

using MovieList.Core;
using MovieList.Core.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class ListItemControlBase : ReactiveUserControl<ListItemViewModel> { }

    public partial class ListItemControl : ListItemControlBase
    {
        private static readonly SolidColorBrush HighlightBrush = new(new Color { A = 255, R = 237, G = 231, B = 246 });

        public ListItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.DisplayNumber, v => v.NumberTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.Title, v => v.TitleTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.OriginalTitle, v => v.OriginalTitleTextBlock.Text)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Item.Year, v => v.YearTextBlock.Text)
                    ?.DisposeWith(disposables);

                var isMouseOver = this.Events().MouseEnter.Select(_ => true)
                    .Merge(this.Events().MouseLeave.Select(_ => false))
                    .DistinctUntilChanged()
                    .StartWith(false);

                this.WhenAnyValue(
                    v => v.ViewModel!.Item.IsHighlighted,
                    v => v.ViewModel!.Item.IsSelected,
                    (highlighted, selected) => highlighted && !selected)
                    .CombineLatest(isMouseOver, (highlight, mouseOver) => highlight && !mouseOver)
                    .Select(highlight => highlight ? HighlightBrush : Brushes.Transparent)
                    .ObserveOnDispatcher()
                    .BindTo(this, v => v.Background)
                    ?.DisposeWith(disposables);

                this.OneWayBind(
                    this.ViewModel,
                    vm => vm.Item.Color,
                    v => v.Foreground,
                    color => new SolidColorBrush
                    {
                        Color = (Color?)ColorConverter.ConvertFromString(color) ?? Colors.Black
                    })
                    ?.DisposeWith(disposables);
            });
        }
    }
}
